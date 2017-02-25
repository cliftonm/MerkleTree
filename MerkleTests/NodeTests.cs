using System;
using System.Text;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Clifton.Blockchain;

namespace MerkleTests
{
    [TestClass]
    public class NodeTests
    {
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
        public void TestCreateBalancedTree()
        {
            MerkleTree tree = new MerkleTree();
            tree.AppendNode(MerkleHash.Create("abc"));
            tree.AppendNode(MerkleHash.Create("def"));
            tree.AppendNode(MerkleHash.Create("123"));
            tree.AppendNode(MerkleHash.Create("456"));
            tree.BuildTree();
            Assert.IsNotNull(tree.RootNode);
        }

        [TestMethod]
        public void TestCreateUnbalancedTree()
        {
            MerkleTree tree = new MerkleTree();
            tree.AppendNode(MerkleHash.Create("abc"));
            tree.AppendNode(MerkleHash.Create("def"));
            tree.AppendNode(MerkleHash.Create("123"));
            tree.BuildTree();
            Assert.IsNotNull(tree.RootNode);
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
