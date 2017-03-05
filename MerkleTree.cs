using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using Clifton.Core.ExtensionMethods;

namespace Clifton.Blockchain
{
    public class MerkleTree : IEnumerable<MerkleNode>
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

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<MerkleNode> GetEnumerator()
        {
            foreach (var n in Iterate(RootNode)) yield return n;
        }

        protected IEnumerable<MerkleNode> Iterate(MerkleNode node)
        {
            yield return node;

            if (node.LeftNode != null)
            {
                foreach (var n in Iterate(node.LeftNode)) yield return n;
            }

            if (node.RightNode != null)
            {
                foreach (var n in Iterate(node.RightNode)) yield return n;
            }
        }

        public void AppendLeaf(MerkleHash hash)
        {
            var node = new MerkleNode(hash);
            nodes.Add(node);
            leaves.Add(node);
        }

        public void AppendLeaves(MerkleHash[] hashes)
        {
            hashes.ForEach(h => AppendLeaf(h));
        }

        public MerkleHash AddTree(MerkleTree tree)
        {
            Contract(() => leaves.Count > 0, "Cannot add to a tree with no leaves.");
            FixOddNumberLeaves();
            tree.leaves.ForEach(l => AppendLeaf(l.Hash));

            return BuildTree();
        }

        /// <summary>
        /// If we have an odd number of leaves, add a leaf that
        /// is a duplicate of the last leaf hash so that when we add the leaves of the new tree,
        /// we don't change the root hash of the current tree.
        /// This is performed on the source tree whenever a tree is added, and can be performed
        /// at any time to ensure an even number of leaves, so that the old root isn't affected,
        /// when adding new leaves.
        /// </summary>
        public void FixOddNumberLeaves()
        {
            if ((leaves.Count & 1) == 1)
            {
                AppendLeaf(leaves.Last().Hash);
            }
        }

        /// <summary>
        /// Builds the tree for leaves and returns the root node.
        /// </summary>
        public MerkleHash BuildTree()
        {
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
        /// Returns the audit trail hashes to reconstruct the root hash.
        /// </summary>
        /// <param name="leafHash">The leaf hash we want to verify exists in the tree.</param>
        /// <returns>The audit trail of hashes needed to create the root, or an empty list if the leaf hash doesn't exist.</returns>
        public List<MerkleAuditHash> Audit(MerkleHash leafHash)
        {
            List<MerkleAuditHash> auditTrail = new List<MerkleAuditHash>();

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
        /// </summary>
        public bool ConsistencyCheck(MerkleHash expectedSubrootHash, int numLeaves)
        {
            MerkleHash[] subtreeLeaves = leaves.Take(numLeaves).Select(n=>n.Hash).ToArray();
            MerkleTree subtree = new MerkleTree();
            subtree.AppendLeaves(subtreeLeaves);
            MerkleHash subtreeHash = subtree.BuildTree();

            return subtreeHash == expectedSubrootHash;
        }

        /// <summary>
        /// Verify that if we walk up the tree from a particular leaf, we encounter the expected root hash.
        /// </summary>
        public static bool VerifyAudit(MerkleHash rootHash, MerkleHash leafHash, List<MerkleAuditHash> auditTrail)
        {
            Contract(() => auditTrail.Count > 0, "Audit trail cannot be empty.");
            MerkleHash testHash = leafHash;

            // TODO: Inefficient - compute hashes directly.
            foreach (MerkleAuditHash auditHash in auditTrail)
            {
                testHash = auditHash.Direction == MerkleAuditHash.Branch.Left ?
                    MerkleHash.Create(testHash.Value.Concat(auditHash.Hash.Value).ToArray()) :
                    MerkleHash.Create(auditHash.Hash.Value.Concat(testHash.Value).ToArray());
            }

            return rootHash == testHash;
        }

        /// <summary>
        /// As an alternate consistency check, we verify that walking up the tree from a particular leaf, we encounter
        /// along the way an "old" root hash.
        /// </summary>
        public static bool VerifyPartialAudit(MerkleHash rootHash, MerkleHash leafHash, List<MerkleAuditHash> auditTrail)
        {
            Contract(() => auditTrail.Count > 0, "Audit trail cannot be empty.");
            bool ret = false;
            MerkleHash testHash = leafHash;

            // TODO: Inefficient - compute hashes directly.
            foreach (MerkleAuditHash auditHash in auditTrail)
            {
                testHash = auditHash.Direction == MerkleAuditHash.Branch.Left ?
                    MerkleHash.Create(testHash.Value.Concat(auditHash.Hash.Value).ToArray()) :
                    MerkleHash.Create(auditHash.Hash.Value.Concat(testHash.Value).ToArray());

                if (rootHash == testHash)
                {
                    ret = true;
                    break;
                }
            }

            return ret;
        }

        protected void BuildAuditTrail(List<MerkleAuditHash> auditTrail, MerkleNode parent, MerkleNode child)
        {
            if (parent != null)
            {
                Contract(() => child.Parent == parent, "Parent of child is not expected parent.");
                var nextChild = parent.LeftNode == child ? parent.RightNode : parent.LeftNode;
                var direction = parent.LeftNode == child ? MerkleAuditHash.Branch.Left : MerkleAuditHash.Branch.Right;
                auditTrail.Add(new MerkleAuditHash(nextChild.Hash, direction));
                BuildAuditTrail(auditTrail, child.Parent.Parent, child.Parent);
            }
        }

        protected MerkleNode FindLeaf(MerkleHash leafHash)
        {
            // TODO: We can improve the search for the leaf hash by maintaining a sorted list of leaf hashes.
            return leaves.SingleOrDefault(l => l.Hash == leafHash);
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
