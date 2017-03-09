using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Clifton.Blockchain;
using Clifton.Core.ExtensionMethods;

namespace MerkleTests
{
    [TestClass]
    public class NodeTests
    {
        [TestMethod]
        public void HashesAreSameTest()
        {
            MerkleHash h1 = MerkleHash.Create("abc");
            MerkleHash h2 = MerkleHash.Create("abc");
            Assert.IsTrue(h1 == h2);
        }

        [TestMethod]
        public void CreateNodeTest()
        {
            MerkleNode node = new MerkleNode();
            Assert.IsNull(node.Parent);
            Assert.IsNull(node.LeftNode);
            Assert.IsNull(node.RightNode);
        }

        /// <summary>
        /// Tests that after setting the left node, the parent hash verifies.
        /// </summary>
        [TestMethod]
        public void LeftHashVerificationTest()
        {
            MerkleNode parentNode = new MerkleNode();
            MerkleNode leftNode = new MerkleNode();
            leftNode.ComputeHash(Encoding.UTF8.GetBytes("abc"));
            parentNode.SetLeftNode(leftNode);
            Assert.IsTrue(parentNode.VerifyHash());
        }

        /// <summary>
        /// Tests that after setting both child nodes (left and right), the parent hash verifies.
        /// </summary>
        [TestMethod]
        public void LeftRightHashVerificationTest()
        {
            MerkleNode parentNode = CreateParentNode("abc", "def");
            Assert.IsTrue(parentNode.VerifyHash());
        }

        [TestMethod]
        public void NodesEqualTest()
        {
            MerkleNode parentNode1 = CreateParentNode("abc", "def");
            MerkleNode parentNode2 = CreateParentNode("abc", "def");
            Assert.IsTrue(parentNode1.Equals(parentNode2));
        }

        [TestMethod]
        public void NodesNotEqualTest()
        {
            MerkleNode parentNode1 = CreateParentNode("abc", "def");
            MerkleNode parentNode2 = CreateParentNode("def", "abc");
            Assert.IsFalse(parentNode1.Equals(parentNode2));
        }

        [TestMethod]
        public void VerifyTwoLevelTree()
        {
            MerkleNode parentNode1 = CreateParentNode("abc", "def");
            MerkleNode parentNode2 = CreateParentNode("123", "456");
            MerkleNode rootNode = new MerkleNode();
            rootNode.SetLeftNode(parentNode1);
            rootNode.SetRightNode(parentNode2);
            Assert.IsTrue(rootNode.VerifyHash());
        }

        [TestMethod]
        public void CreateBalancedTreeTest()
        {
            MerkleTree tree = new MerkleTree();
            tree.AppendLeaf(MerkleHash.Create("abc"));
            tree.AppendLeaf(MerkleHash.Create("def"));
            tree.AppendLeaf(MerkleHash.Create("123"));
            tree.AppendLeaf(MerkleHash.Create("456"));
            tree.BuildTree();
            Assert.IsNotNull(tree.RootNode);
        }

        [TestMethod]
        public void CreateUnbalancedTreeTest()
        {
            MerkleTree tree = new MerkleTree();
            tree.AppendLeaf(MerkleHash.Create("abc"));
            tree.AppendLeaf(MerkleHash.Create("def"));
            tree.AppendLeaf(MerkleHash.Create("123"));
            tree.BuildTree();
            Assert.IsNotNull(tree.RootNode);
        }

        // A Merkle audit path for a leaf in a Merkle Hash Tree is the shortest
        // list of additional nodes in the Merkle Tree required to compute the
        // Merkle Tree Hash for that tree.
        [TestMethod]
        public void AuditTest()
        {
            // Build a tree, and given the root node and a leaf hash, verify that the we can reconstruct the root hash.
            MerkleTree tree = new MerkleTree();
            MerkleHash l1 = MerkleHash.Create("abc");
            MerkleHash l2 = MerkleHash.Create("def");
            MerkleHash l3 = MerkleHash.Create("123");
            MerkleHash l4 = MerkleHash.Create("456");
            tree.AppendLeaves(new MerkleHash[] { l1, l2, l3, l4 });
            MerkleHash rootHash = tree.BuildTree();

            List<MerkleProofHash> auditTrail = tree.AuditProof(l1);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l1, auditTrail));

            auditTrail = tree.AuditProof(l2);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l2, auditTrail));

            auditTrail = tree.AuditProof(l3);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l3, auditTrail));

            auditTrail = tree.AuditProof(l4);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l4, auditTrail));
        }

        [TestMethod]
        public void FixingOddNumberOfLeavesByAddingTreeTest()
        {
            MerkleTree tree = new MerkleTree();
            MerkleHash l1 = MerkleHash.Create("abc");
            MerkleHash l2 = MerkleHash.Create("def");
            MerkleHash l3 = MerkleHash.Create("123");
            tree.AppendLeaves(new MerkleHash[] { l1, l2, l3 });
            MerkleHash rootHash = tree.BuildTree();
            tree.AddTree(new MerkleTree());
            MerkleHash rootHashAfterFix = tree.BuildTree();
            Assert.IsTrue(rootHash == rootHashAfterFix);
        }

        [TestMethod]
        public void FixingOddNumberOfLeavesManuallyTest()
        {
            MerkleTree tree = new MerkleTree();
            MerkleHash l1 = MerkleHash.Create("abc");
            MerkleHash l2 = MerkleHash.Create("def");
            MerkleHash l3 = MerkleHash.Create("123");
            tree.AppendLeaves(new MerkleHash[] { l1, l2, l3 });
            MerkleHash rootHash = tree.BuildTree();
            tree.FixOddNumberLeaves();
            MerkleHash rootHashAfterFix = tree.BuildTree();
            Assert.IsTrue(rootHash != rootHashAfterFix);
        }

        [TestMethod]
        public void AddTreeTest()
        {
            MerkleTree tree = new MerkleTree();
            MerkleHash l1 = MerkleHash.Create("abc");
            MerkleHash l2 = MerkleHash.Create("def");
            MerkleHash l3 = MerkleHash.Create("123");
            tree.AppendLeaves(new MerkleHash[] { l1, l2, l3 });
            MerkleHash rootHash = tree.BuildTree();

            MerkleTree tree2 = new MerkleTree();
            MerkleHash l5 = MerkleHash.Create("456");
            MerkleHash l6 = MerkleHash.Create("xyzzy");
            MerkleHash l7 = MerkleHash.Create("fizbin");
            MerkleHash l8 = MerkleHash.Create("foobar");
            tree2.AppendLeaves(new MerkleHash[] { l5, l6, l7, l8 });
            MerkleHash tree2RootHash = tree2.BuildTree();
            MerkleHash rootHashAfterAddTree = tree.AddTree(tree2);

            Assert.IsTrue(rootHash != rootHashAfterAddTree);
        }

        // Merkle consistency proofs prove the append-only property of the tree.
        [TestMethod]
        public void ConsistencyTest()
        {
            // Start with a tree with 2 leaves:
            MerkleTree tree = new MerkleTree();
            var startingNodes = tree.AppendLeaves(new MerkleHash[]
                {
                    MerkleHash.Create("1"),
                    MerkleHash.Create("2"),
                });

            startingNodes.ForEachWithIndex((n, i) => n.Text = i.ToString());

            MerkleHash firstRoot = tree.BuildTree();

            List<MerkleHash> oldRoots = new List<MerkleHash>() { firstRoot };

            // Add a new leaf and verify that each time we add a leaf, we can get a consistency check
            // for all the previous leaves.
            for (int i = 2; i < 100; i++)
            {
                tree.AppendLeaf(MerkleHash.Create(i.ToString())).Text=i.ToString();
                tree.BuildTree();

                // After adding a leaf, verify that all the old root hashes exist.
                oldRoots.ForEachWithIndex((oldRootHash, n) =>
                {
                    List<MerkleProofHash> proof = tree.ConsistencyProof(n+2);
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

                    Assert.IsTrue(hash == oldRootHash, "Old root hash not found for index " + i + " m = " + (n+2).ToString());
                    
                });

                // Then we add this root hash as the next old root hash to check.
                oldRoots.Add(tree.RootNode.Hash);
            }
        }

        private MerkleNode CreateParentNode(string leftData, string rightData)
        {
            MerkleNode parentNode = new MerkleNode();
            MerkleNode leftNode = new MerkleNode();
            MerkleNode rightNode = new MerkleNode();
            leftNode.ComputeHash(Encoding.UTF8.GetBytes(leftData));
            rightNode.ComputeHash(Encoding.UTF8.GetBytes(rightData));
            parentNode.SetLeftNode(leftNode);
            parentNode.SetRightNode(rightNode);

            return parentNode;
        }
    }
}
