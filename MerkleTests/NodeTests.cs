using System.Collections.Generic;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Clifton.Blockchain;

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

            List<MerkleAuditHash> auditTrail = tree.Audit(l1);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l1, auditTrail));

            auditTrail = tree.Audit(l2);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l2, auditTrail));

            auditTrail = tree.Audit(l3);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l3, auditTrail));

            auditTrail = tree.Audit(l4);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l4, auditTrail));
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
