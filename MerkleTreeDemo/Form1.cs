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
        public const int NUM_LEAVES = 8;
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
            CreateTree(tree);
            DrawTree(tree);
            DrawAuditProof("4", tree);
        }

        public void ConsistencyProofDemo()
        {
            MerkleTree hashTree0 = new MerkleTree();
            hashTree0.AppendLeaves(new MerkleNode[]
                {
                    new MerkleNode(MerkleHash.Create("0")).SetText("0"),
                    new MerkleNode(MerkleHash.Create("1")).SetText("1"),
                    new MerkleNode(MerkleHash.Create("2")).SetText("2"),
                });
            MerkleHash hash0 = hashTree0.BuildTree();

            MerkleTree hashTree1 = new MerkleTree();
            hashTree1.AppendLeaves(new MerkleNode[]
                {
                    new MerkleNode(MerkleHash.Create("3")).SetText("3"),
                });
            MerkleHash hash1 = hashTree1.BuildTree();
            hashTree0.AddTree(hashTree1);

            MerkleTree hashTree2 = new MerkleTree();
            hashTree2.AppendLeaves(new MerkleNode[]
                {
                    new MerkleNode(MerkleHash.Create("4")).SetText("4"),
                    new MerkleNode(MerkleHash.Create("5")).SetText("5"),
                });
            MerkleHash hash2 = hashTree2.BuildTree();
            hashTree0.AddTree(hashTree2);

            MerkleTree hashTree3 = new MerkleTree();
            hashTree3.AppendLeaves(new MerkleNode[]
                {
                    new MerkleNode(MerkleHash.Create("6")).SetText("6"),
                    new MerkleNode(MerkleHash.Create("7")).SetText("7"),
                });
            MerkleHash rootHash = hashTree0.AddTree(hashTree3);

            DrawTree(hashTree0);

            // hashTree0 is now the full tree.
            // In this case, we're providing the m-value for clarity of the demo.
            // See diagram in section 2.1.3 of https://tools.ietf.org/html/rfc6962
            //DrawConsistencyProof(hashTree0, hash0, 3, rootHash);
            // DrawConsistencyProof(hashTree0, hash1, 4, rootHash);
            // DrawConsistencyProof(hashTree0, hash2, 6, rootHash);
            DrawConsistencyProof(hashTree0, hash2, 7, rootHash);
        }

        public void CreateTree(MerkleTree tree)
        {
            List<MerkleNode> leaves = new List<MerkleNode>();

            for (int i = 0; i < NUM_LEAVES; i++)
            {
                var node = tree.AppendLeaf(MerkleHash.Create(i.ToString()));
                node.Text = i.ToString();
            }

            tree.BuildTree();
        }

        public void DrawTree(MerkleTree tree)
        {
            List<MerkleNode> leaves = leaves = tree.RootNode.Leaves().ToList();
            List<Rectangle> shapesLower = DrawLeaves(leaves);
            IEnumerable<MerkleNode> parents = leaves.Select(l => l.Parent).Distinct();

            int level = 1;

            while (parents.Count() > 0)
            {
                List<Rectangle> shapesUpper = DrawParents(parents, level++);
                DrawConnectors(shapesLower, shapesUpper);
                shapesLower = shapesUpper;
                parents = parents.Select(p => p?.Parent).Where(p=>p != null).Distinct();
                // CreateParentTags(parents);
            }
        }

        protected void DrawAuditProof(string text, MerkleTree tree)
        {
            // We use First because a tree with an odd number of leaves will duplicate the last leaf
            // when computing the hash.
            MerkleNode node = tree.RootNode.First(t => t.Text == text);
            List<MerkleAuditHash> proof = tree.Audit(node.Hash);

            foreach (var auditHash in proof)
            {
                // We use First because a tree with an odd number of leaves will duplicate the last leaf
                // when computing the hash.
                MerkleNode n = tree.RootNode.First(t => t.Hash == auditHash.Hash);
                Highlight(n);
            }
        }

        protected void DrawConsistencyProof(MerkleTree tree, MerkleHash originalDataHash, int m, MerkleHash newRootHash)
        {
            List<MerkleNode> proof = tree.ConsistencyCheck(m);
            proof.ForEach(node => Highlight(node));
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



