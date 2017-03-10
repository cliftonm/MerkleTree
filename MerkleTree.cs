using System;
using System.Collections.Generic;
using System.Linq;

using Clifton.Core.ExtensionMethods;

namespace Clifton.Blockchain
{
    public class MerkleTree
    {
        public MerkleNode RootNode { get; protected set; }

        protected List<MerkleNode> nodes;
        protected List<MerkleNode> leaves;

        public static void Contract(Func<bool> action, string msg)
        {
            if (!action())
            {
                throw new MerkleException(msg);
            }
        }

        public MerkleTree()
        {
            nodes = new List<MerkleNode>();
            leaves = new List<MerkleNode>();
        }

        public MerkleNode AppendLeaf(MerkleNode node)
        {
            nodes.Add(node);
            leaves.Add(node);

            return node;
        }

        public void AppendLeaves(MerkleNode[] nodes)
        {
            nodes.ForEach(n => AppendLeaf(n));
        }

        public MerkleNode AppendLeaf(MerkleHash hash)
        {
            var node = new MerkleNode(hash);
            nodes.Add(node);
            leaves.Add(node);

            return node;
        }

        public List<MerkleNode> AppendLeaves(MerkleHash[] hashes)
        {
            List<MerkleNode> nodes = new List<MerkleNode>();
            hashes.ForEach(h => nodes.Add(AppendLeaf(h)));

            return nodes;
        }

        public MerkleHash AddTree(MerkleTree tree)
        {
            Contract(() => leaves.Count > 0, "Cannot add to a tree with no leaves.");
            tree.leaves.ForEach(l => AppendLeaf(l));

            return BuildTree();
        }

        /// <summary>
        /// If we have an odd number of leaves, add a leaf that
        /// is a duplicate of the last leaf hash so that when we add the leaves of the new tree,
        /// we don't change the root hash of the current tree.
        /// This method should only be used if you have a specific reason that you need to balance
        /// the last node with it's right branch, for example as a pre-step to computing an audit trail
        /// on the last leaf of an odd number of leaves in the tree.
        /// </summary>
        public void FixOddNumberLeaves()
        {
            if ((leaves.Count & 1) == 1)
            {
                var lastLeaf = leaves.Last();
                var l = AppendLeaf(lastLeaf.Hash);
                l.Text = lastLeaf.Text;
            }
        }

        /// <summary>
        /// Builds the tree for leaves and returns the root node.
        /// </summary>
        public MerkleHash BuildTree()
        {
            // We do not call FixOddNumberLeaves because we want the ability to append 
            // leaves and add additional trees without creating unecessary wasted space in the tree.
            Contract(() => leaves.Count > 0, "Cannot build a tree with no leaves.");
            BuildTree(leaves);

            return RootNode.Hash;
        }

        // Why would we need this?
        //public void RegisterRoot(MerkleNode node)
        //{
        //    Contract(() => node.Parent == null, "Node is not a root node.");
        //    rootNode = node;
        //}

        /// <summary>
        /// Returns the audit proof hashes to reconstruct the root hash.
        /// </summary>
        /// <param name="leafHash">The leaf hash we want to verify exists in the tree.</param>
        /// <returns>The audit trail of hashes needed to create the root, or an empty list if the leaf hash doesn't exist.</returns>
        public List<MerkleProofHash> AuditProof(MerkleHash leafHash)
        {
            List<MerkleProofHash> auditTrail = new List<MerkleProofHash>();

            var leafNode = FindLeaf(leafHash);

            if (leafNode != null)
            {
                Contract(() => leafNode.Parent != null, "Expected leaf to have a parent.");
                var parent = leafNode.Parent;
                BuildAuditTrail(auditTrail, parent, leafNode);
            }

            return auditTrail;
        }

        /// <summary>
        /// Verifies ordering and consistency of the first n leaves, such that we reach the expected subroot.
        /// This verifies that the prior data has not been changed and that leaf order has been preserved.
        /// m is the number of leaves for which to do a consistency check.
        /// </summary>
        public List<MerkleProofHash> ConsistencyProof(int m)
        {
            // Rule 1:
            // Find the leftmost node of the tree from which we can start our consistency proof.
            // Set k, the number of leaves for this node.
            List<MerkleProofHash> hashNodes = new List<MerkleProofHash>();
            int idx = (int)Math.Log(m, 2);

            // Get the leftmost node.
            MerkleNode node = leaves[0];

            // Traverse up the tree until we get to the node specified by idx.
            while (idx > 0)
            {
                node = node.Parent;
                --idx;
            }

            int k = node.Leaves().Count();
            hashNodes.Add(new MerkleProofHash(node.Hash, MerkleProofHash.Branch.OldRoot));

            if (m == k)
            {
                // Continue with Rule 3 -- the remainder is the audit proof
            }
            else
            {
                // Rule 2:
                // Set the initial sibling node (SN) to the sibling of the node acquired by Rule 1.
                // if m-k == # of SN's leaves, concatenate the hash of the sibling SN and exit Rule 2, as this represents the hash of the old root.
                // if m - k < # of SN's leaves, set SN to SN's left child node and repeat Rule 2.

                // sibling node:
                MerkleNode sn = node.Parent.RightNode;
                bool traverseTree = true;

                while (traverseTree)
                {
                    Contract(() => sn != null, "Sibling node must exist because m != k");
                    int sncount = sn.Leaves().Count();

                    if (m - k == sncount)
                    {
                        hashNodes.Add(new MerkleProofHash(sn.Hash, MerkleProofHash.Branch.OldRoot));
                        break;
                    }

                    if (m - k > sncount)
                    {
                        hashNodes.Add(new MerkleProofHash(sn.Hash, MerkleProofHash.Branch.OldRoot));
                        sn = sn.Parent.RightNode;
                        k += sncount;
                    }
                    else // (m - k < sncount)
                    {
                        sn = sn.LeftNode;
                    }
                }
            }

            // Rule 3: Apply ConsistencyAuditProof below.

            return hashNodes;
        }

        /// <summary>
        /// Completes the consistency proof with an audit proof using the last node in the consistency proof.
        /// </summary>
        public List<MerkleProofHash> ConsistencyAuditProof(MerkleHash nodeHash)
        {
            List<MerkleProofHash> auditTrail = new List<MerkleProofHash>();

            var node = RootNode.Single(n => n.Hash == nodeHash);
            var parent = node.Parent;
            BuildAuditTrail(auditTrail, parent, node);

            return auditTrail;
        }

        /// <summary>
        /// Verify that if we walk up the tree from a particular leaf, we encounter the expected root hash.
        /// </summary>
        public static bool VerifyAudit(MerkleHash rootHash, MerkleHash leafHash, List<MerkleProofHash> auditTrail)
        {
            Contract(() => auditTrail.Count > 0, "Audit trail cannot be empty.");
            MerkleHash testHash = leafHash;

            // TODO: Inefficient - compute hashes directly.
            foreach (MerkleProofHash auditHash in auditTrail)
            {
                testHash = auditHash.Direction == MerkleProofHash.Branch.Left ?
                    MerkleHash.Create(testHash.Value.Concat(auditHash.Hash.Value).ToArray()) :
                    MerkleHash.Create(auditHash.Hash.Value.Concat(testHash.Value).ToArray());
            }

            return rootHash == testHash;
        }

        /// <summary>
        /// For demo / debugging purposes, we return the pairs of hashes used to verify the audit proof.
        /// </summary>
        public static List<Tuple<MerkleHash, MerkleHash>> AuditHashPairs(MerkleHash leafHash, List<MerkleProofHash> auditTrail)
        {
            Contract(() => auditTrail.Count > 0, "Audit trail cannot be empty.");
            var auditPairs = new List<Tuple<MerkleHash, MerkleHash>>();
            MerkleHash testHash = leafHash;

            // TODO: Inefficient - compute hashes directly.
            foreach (MerkleProofHash auditHash in auditTrail)
            {
                switch (auditHash.Direction)
                {
                    case MerkleProofHash.Branch.Left:
                        auditPairs.Add(new Tuple<MerkleHash, MerkleHash>(testHash, auditHash.Hash));
                        testHash = MerkleHash.Create(testHash.Value.Concat(auditHash.Hash.Value).ToArray());
                        break;

                    case MerkleProofHash.Branch.Right:
                        auditPairs.Add(new Tuple<MerkleHash, MerkleHash>(auditHash.Hash, testHash));
                        testHash = MerkleHash.Create(auditHash.Hash.Value.Concat(testHash.Value).ToArray());
                        break;
                }
            }

            return auditPairs;
        }

        public static bool VerifyConsistency(MerkleHash oldRootHash, List<MerkleProofHash> proof)
        {
            MerkleHash hash, lhash, rhash;

            if (proof.Count > 1)
            {
                lhash = proof[proof.Count - 2].Hash;
                int hidx = proof.Count - 1;
                hash = rhash = MerkleTree.ComputeHash(lhash, proof[hidx].Hash);
                hidx -= 2;

                // foreach (var nextHashNode in proof.Skip(1))
                while (hidx >= 0)
                {
                    lhash = proof[hidx].Hash;
                    hash = rhash = MerkleTree.ComputeHash(lhash, rhash);

                    --hidx;
                }
            }
            else
            {
                hash = proof[0].Hash;
            }

            return hash == oldRootHash;
        }

        public static MerkleHash ComputeHash(MerkleHash left, MerkleHash right)
        {
            return MerkleHash.Create(left.Value.Concat(right.Value).ToArray());
        }

        protected void BuildAuditTrail(List<MerkleProofHash> auditTrail, MerkleNode parent, MerkleNode child)
        {
            if (parent != null)
            {
                Contract(() => child.Parent == parent, "Parent of child is not expected parent.");
                var nextChild = parent.LeftNode == child ? parent.RightNode : parent.LeftNode;
                var direction = parent.LeftNode == child ? MerkleProofHash.Branch.Left : MerkleProofHash.Branch.Right;

                // For the last leaf, the right node may not exist.  In that case, we ignore it because it's
                // the hash we are given to verify.
                if (nextChild != null)
                {
                    auditTrail.Add(new MerkleProofHash(nextChild.Hash, direction));
                }

                BuildAuditTrail(auditTrail, child.Parent.Parent, child.Parent);
            }
        }

        protected MerkleNode FindLeaf(MerkleHash leafHash)
        {
            // TODO: We can improve the search for the leaf hash by maintaining a sorted list of leaf hashes.
            // We use First because a tree with an odd number of leaves will duplicate the last leaf
            // and will therefore have the same hash.
            return leaves.FirstOrDefault(l => l.Hash == leafHash);
        }

        /// <summary>
        /// Reduce the current list of n nodes to n/2 parents.
        /// </summary>
        /// <param name="nodes"></param>
        protected void BuildTree(List<MerkleNode> nodes)
        {
            Contract(() => nodes.Count > 0, "node list not expected to be empty.");

            if (nodes.Count == 1)
            {
                RootNode = nodes[0];
            }
            else
            {
                List<MerkleNode> parents = new List<MerkleNode>();

                for (int i = 0; i < nodes.Count; i += 2)
                {
                    MerkleNode right = (i + 1 < nodes.Count) ? nodes[i + 1] : null;
                    // Constructing the MerkleNode resolves the right node being null.
                    MerkleNode parent = new MerkleNode(nodes[i], right);
                    parents.Add(parent);
                }

                BuildTree(parents);
            }
        }
    }
}
