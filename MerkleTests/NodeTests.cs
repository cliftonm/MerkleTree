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

            List<MerkleAuditHash> auditTrail = tree.Audit(l1);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l1, auditTrail));

            auditTrail = tree.Audit(l2);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l2, auditTrail));

            auditTrail = tree.Audit(l3);
            Assert.IsTrue(MerkleTree.VerifyAudit(rootHash, l3, auditTrail));

            auditTrail = tree.Audit(l4);
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
            Assert.IsTrue(rootHash == rootHashAfterFix);
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
            MerkleTree tree = new MerkleTree();
            MerkleHash l1 = MerkleHash.Create("abc");
            MerkleHash l2 = MerkleHash.Create("def");
            MerkleHash l3 = MerkleHash.Create("123");
            MerkleHash l4 = MerkleHash.Create("456");
            tree.AppendLeaves(new MerkleHash[] { l1, l2, l3, l4 });
            MerkleHash rootHash = tree.BuildTree();

            // See diagrams here: https://www.certificate-transparency.org/log-proofs-work
            // Append a couple certificates, creating "k"
            MerkleTree tree2 = new MerkleTree();
            MerkleHash l5 = MerkleHash.Create("7890");
            MerkleHash l6 = MerkleHash.Create("xyzzy");
            tree2.AppendLeaves(new MerkleHash[] { l5, l6 });
            tree.BuildTree();
            MerkleHash tree2RootHash = tree2.BuildTree();
            MerkleHash rootHashAfterAddTree = tree.AddTree(tree2);

            // Deviating from the document linked above:
            // At this point, m and k should exist and their combined hash should equal rootHashAfterAddTree
            // We can verify this a couple of ways, but we'll do this by doing an audit, not on a leaf, but on the old root hash (rootHash) and the root hash
            // of the tree we just added (tree2RootHash).

            // From: http://www.links.org/files/sunlight.html (2.1.2)
            // This approach is different from the discussion in the "log-proofs-work" document and seems more robust.
            // Merkle consistency proofs prove the append-only property of the tree. A Merkle consistency proof for a Merkle Tree Hash MTH(D[0:n]) and a previously advertised hash MTH(D[0:m]) of the first m leaves, m <= n, is the list of nodes in the Merkle tree required to verify that the first m inputs D[0:m] are equal in both trees. Thus, a consistency proof must contain a set of intermediate nodes (i.e., commitments to inputs) sufficient to verify MTH(D[0:n]), such that (a subset of) the same nodes can be used to verify MTH(D[0:m]). We define an algorithm that outputs the (unique) minimal consistency proof.
            // Given an ordered list of n inputs to the tree, D[0:n] = {d(0), ..., d(n-1)}, the Merkle consistency proof PROOF(m, D[0:n]) for a previous root hash MTH(D[0:m]), 0 < m < n, is defined as PROOF(m, D[0:n]) = SUBPROOF(m, D[0:n], true):

            // These tests verify that order has been preserved and that intermediate hashes have not been changed.
            // The first 4 leaves should give us the same rootHash as the original tree.
            // We know that rootHash is associated with the first 4 leaves.
            Assert.IsTrue(tree.ConsistencyCheck(rootHash, 4));

            // Now append two more certificates, creating l, which create n and updates the main tree root.
            MerkleHash l7 = MerkleHash.Create("fizbin");
            MerkleHash l8 = MerkleHash.Create("foobar");
            tree.AppendLeaves(new MerkleHash[] { l7, l8 });
            tree.BuildTree();

            // The first 6 leaves should give us the same rootHash as the root hash we got with after the first append of two leaves (rootHashAfterAddTree) 
            // *after* we appended the next (and last) 2 leaves.
            // We know that rootHashAfterAddTree is associated with the first 6 leaves.
            Assert.IsTrue(tree.ConsistencyCheck(rootHashAfterAddTree, 6));
        }

        [TestMethod]
        public void AlternateConsistencyTest()
        {
            // An alternate approach is, if we know the hash of one of the leaves in the original tree, we should encounter the
            // old root hash in the audit trail.

            MerkleTree tree = new MerkleTree();
            MerkleHash l1 = MerkleHash.Create("abc");
            MerkleHash l2 = MerkleHash.Create("def");
            MerkleHash l3 = MerkleHash.Create("123");
            MerkleHash l4 = MerkleHash.Create("456");
            tree.AppendLeaves(new MerkleHash[] { l1, l2, l3, l4 });
            MerkleHash rootHash = tree.BuildTree();

            // See diagrams here: https://www.certificate-transparency.org/log-proofs-work
            // Append a couple certificates, creating "k"
            MerkleTree tree2 = new MerkleTree();
            MerkleHash l5 = MerkleHash.Create("7890");
            MerkleHash l6 = MerkleHash.Create("xyzzy");
            tree2.AppendLeaves(new MerkleHash[] { l5, l6 });
            tree.AddTree(tree2);

            MerkleHash tree2RootHash = tree2.BuildTree();

            MerkleHash l7 = MerkleHash.Create("fizbin");
            MerkleHash l8 = MerkleHash.Create("foobar");
            tree.AppendLeaves(new MerkleHash[] { l7, l8 });
            tree.BuildTree();

            List<MerkleAuditHash> auditTrail = tree.Audit(l1);
            Assert.IsTrue(MerkleTree.VerifyPartialAudit(rootHash, l1, auditTrail));

            auditTrail = tree.Audit(l5);
            Assert.IsTrue(MerkleTree.VerifyPartialAudit(tree2RootHash, l5, auditTrail));
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
