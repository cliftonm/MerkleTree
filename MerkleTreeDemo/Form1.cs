using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using FlowSharpLib;

using Clifton.Core.ExtensionMethods;
using Clifton.Blockchain;

namespace MerkleTreeDemo
{
    public partial class Form1 : Form
    {
        public const int LEAF_Y = 300;
        public const int V_OFFSET = 60;
        public const int X_OFFSET = 80;
        public const int NODE_WIDTH = 75;
        public const int NODE_HEIGHT = 30;
        public const int NUM_LEAVES = 7;
        public readonly Color LEAF_COLOR = Color.LightGreen;
        public readonly Color NODE_COLOR = Color.LightBlue;
        public readonly Color ROOT_COLOR = ControlPaint.LightLight(Color.Red);

        public Form1()
        {
            InitializeComponent();
            AuditProofDemo();

            // ConsistencyProofDemo();
        }

        public void AuditProofDemo()
        {
            MerkleTree tree = new MerkleTree();
            DrawBasicTree(tree);
            // DrawAuditProof("2", tree);
        }

        public void ConsistencyProofDemo()
        {
            MerkleTree hashTree0 = new MerkleTree();
            hashTree0.AppendLeaves(new MerkleHash[]
                {
                    MerkleHash.Create("0"),
                    MerkleHash.Create("1"),
                    MerkleHash.Create("2"),
                });
            MerkleHash hash0 = hashTree0.BuildTree();

            MerkleTree hashTree1 = new MerkleTree();
            hashTree1.AppendLeaves(new MerkleHash[]
                {
                    MerkleHash.Create("3"),
                });
            MerkleHash hash1 = hashTree1.BuildTree();
            hashTree0.AddTree(hashTree1);

            MerkleTree hashTree2 = new MerkleTree();
            hashTree2.AppendLeaves(new MerkleHash[]
                {
                    MerkleHash.Create("4"),
                    MerkleHash.Create("5"),
                });
            MerkleHash hash2 = hashTree2.BuildTree();
            hashTree0.AddTree(hashTree2);

            MerkleTree hashTree3 = new MerkleTree();
            hashTree3.AppendLeaves(new MerkleHash[]
                {
                    MerkleHash.Create("6"),
                });
            MerkleHash rootHash = hashTree0.AddTree(hashTree3);

            // hashTree0 is now the full tree.
            // In this case, we're providing the m-value for clarity of the demo.
            // See diagram in section 2.1.3 of https://tools.ietf.org/html/rfc6962
            DrawConsistencyProof(hashTree0, hash0, 3, rootHash);
            DrawConsistencyProof(hashTree0, hash1, 4, rootHash);
            DrawConsistencyProof(hashTree0, hash2, 6, rootHash);
        }

        public void DrawBasicTree(MerkleTree tree)
        {
            for (int i = 0; i < NUM_LEAVES; i++)
            {
                tree.AppendLeaf(MerkleHash.Create(i.ToString()));
            }

            tree.BuildTree();

            MerkleNode rootNode = tree.RootNode;

            // Get all leaves
            List<MerkleNode> leaves = new List<MerkleNode>();
            GetLeaves(rootNode, leaves);
            leaves.ForEachWithIndex((idx, l) => l.Text = idx.ToString());

            List<Rectangle> shapesLower = DrawLeaves(leaves);

            IEnumerable<MerkleNode> parents = leaves.Select(l => l.Parent).Distinct();
            CreateParentTags(parents);

            int level = 1;

            while (parents.Count() > 0)
            {
                List<Rectangle> shapesUpper = DrawParents(parents, level++);
                DrawConnectors(shapesLower, shapesUpper);
                shapesLower = shapesUpper;
                parents = parents.Select(p => p?.Parent).Where(p=>p != null).Distinct();
                CreateParentTags(parents);
            }
        }

        protected void DrawAuditProof(string text, MerkleTree tree)
        {
            MerkleNode node = tree.RootNode.Single(t => t.Text == text);
            List<MerkleAuditHash> proof = tree.Audit(node.Hash);

            foreach (var auditHash in proof)
            {
                MerkleNode n = tree.RootNode.Single(t => t.Hash == auditHash.Hash);
                Highlight(n);
            }
        }

        protected void DrawConsistencyProof(MerkleTree tree, MerkleHash originalDataHash, int m, MerkleHash newRootHash)
        {
        }

        protected void CreateParentTags(IEnumerable<MerkleNode> parents)
        {
            parents.ForEach(p => p.Text = p.LeftNode.Text + p?.RightNode?.Text ?? "-");
        }

        protected void GetLeaves(MerkleNode node, List<MerkleNode> leaves)
        {
            if (node.LeftNode == null && node.RightNode == null)
            {
                leaves.Add(node);
            }
            else
            {
                // Left node will always exist.
                GetLeaves(node.LeftNode, leaves);

                // Right node may not exist and end of leaves.
                if (node.RightNode != null)
                {
                    GetLeaves(node.RightNode, leaves);
                }
            }
        }

        protected List<Rectangle> DrawLeaves(List<MerkleNode> leaves)
        {
            List<Rectangle> rects = new List<Rectangle>();
            int i = 0;

            foreach (var leaf in leaves)
            {
                var leafRect = new Rectangle(i * X_OFFSET, LEAF_Y, NODE_WIDTH, NODE_HEIGHT);
                WebSocketHelpers.DropShape("Box", leaf.Text, leafRect, LEAF_COLOR, leaf.Text);
                rects.Add(leafRect);
                leaf.Tag = leafRect;
                ++i;
            }

            return rects;
        }

        protected List<Rectangle> DrawParents(IEnumerable<MerkleNode> parents, int level)
        {
            List<Rectangle> rects = new List<Rectangle>();
            int i = 0;
            int l0 = level - 1;
            int indent = ((int)Math.Pow(2, l0) - 1) * X_OFFSET + X_OFFSET / 2;
            int spacing = X_OFFSET * (int)Math.Pow(2, level);
            Color nodeColor = parents.Count() == 1 ? ROOT_COLOR : NODE_COLOR;

            foreach (var node in parents)
            {
                var nodeRect = new Rectangle(indent + i * spacing, LEAF_Y - (V_OFFSET * level), NODE_WIDTH, NODE_HEIGHT);
                WebSocketHelpers.DropShape("Box", node.Text, nodeRect, nodeColor, node.Text);
                rects.Add(nodeRect);
                node.Tag = nodeRect;
                ++i;
            }

            return rects;
        }

        protected void DrawConnectors(List<Rectangle> shapesLower, List<Rectangle> shapesUpper)
        {
            int n = 0;
            int n2 = 0;

            foreach (Rectangle rlower in shapesLower)
            {
                Rectangle rupper = shapesUpper[n2];
                var topMiddle = rlower.TopMiddle();
                var bottomMiddle = rupper.BottomMiddle();
                // WebSocketHelpers.DropConnector("DiagonalConnector", bottomMiddle.X, bottomMiddle.Y, topMiddle.X, topMiddle.Y);
                WebSocketHelpers.DropConnector("DynamicConnectorUD", "", bottomMiddle.X, bottomMiddle.Y, topMiddle.X, topMiddle.Y);
                n++;
                n2 = n2 + ((n % 2) == 0 ? 1 : 0);
            }
        }

        protected void Highlight(MerkleNode node)
        {
            Rectangle r = (Rectangle)node.Tag;
            WebSocketHelpers.UpdateProperty(node.Text, "FillColor", "Yellow");
        }
    }
}



