/* 
* Copyright (c) Marc Clifton
* The Code Project Open License (CPOL) 1.02
* http://www.codeproject.com/info/cpol10.aspx
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using FlowSharpLib;
using FlowSharpServiceInterfaces;

using Clifton.Core.ExtensionMethods;
using Clifton.Blockchain;

namespace MerkleTreeDemo
{
    public partial class Demo : Form
    {
        public const int LEAF_Y = 300;
        public const int V_OFFSET = 60;
        public const int X_OFFSET = 80;
        public const int LEAF_WIDTH = 75;
        public const int NODE_WIDTH = 95;
        public const int NODE_HEIGHT = 30;
        public const string CRLF = "\r\n";
        public readonly Color LEAF_COLOR = Color.LightGreen;
        public readonly Color NODE_COLOR = Color.LightBlue;
        public readonly Color ROOT_COLOR = ControlPaint.LightLight(Color.Red);

        public Demo()
        {
            InitializeComponent();
            InitializeFlowSharp();
            Shown += OnShown;
        }

        protected void InitializeFlowSharp()
        {
            var canvasService = Program.ServiceManager.Get<IFlowSharpCanvasService>();
            canvasService.CreateCanvas(pnlFlowSharp);
            canvasService.ActiveController.Canvas.EndInit();
            canvasService.ActiveController.Canvas.Invalidate();

            // Initialize Toolbox so we can drop shapes
            IFlowSharpToolboxService toolboxService = Program.ServiceManager.Get<IFlowSharpToolboxService>();

            // We don't display the toolbox, but we need a container.
            Panel pnlToolbox = new Panel();
            pnlToolbox.Visible = false;
            Controls.Add(pnlToolbox);

            toolboxService.CreateToolbox(pnlToolbox);
            toolboxService.InitializeToolbox();

            var mouseController = Program.ServiceManager.Get<IFlowSharpMouseControllerService>();
            mouseController.Initialize(canvasService.ActiveController);
        }

        private void OnShown(object sender, EventArgs e)
        {
            WebSocketHelpers.ClearCanvas();
            MerkleTree tree = new DemoMerkleTree();
            CreateTree(tree, (int)nudNumLeaves.Value);
            DrawTree(tree);
        }

        public void CreateTree(MerkleTree tree, int numLeaves)
        {
            List<DemoMerkleNode> leaves = new List<DemoMerkleNode>();

            for (int i = 0; i < numLeaves; i++)
            {
                tree.AppendLeaf(DemoMerkleNode.Create(i.ToString()).SetText(i.ToString("X")));
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
            }
        }

        protected void DrawAuditProof(string text, MerkleTree tree)
        {
            // We use First because a tree with an odd number of leaves will duplicate the last leaf
            // when computing the hash.
            var node = tree.RootNode.Cast<DemoMerkleNode>().First(t => t.Text == text);
            Highlight(node, "Orange");
            List<MerkleProofHash> proof = tree.AuditProof(node.Hash);
            bool ret = MerkleTree.VerifyAudit(tree.RootNode.Hash, node.Hash, proof);
            lblAuditPassFail.Text = ret ? "Pass" : "Fail";
            lblAuditPassFail.ForeColor = ret ? Color.Green : Color.Red;
            tbAuditTrail.Clear();

            foreach (var auditHash in proof)
            {
                // We use First because a tree with an odd number of leaves will duplicate the last leaf
                // when computing the hash.
                var n = tree.RootNode.Cast<DemoMerkleNode>().First(t => t.Hash == auditHash.Hash);
                // tbAuditTrail.AppendText(n.Text + CRLF);
                Highlight(n);
            }

            var auditPairs = MerkleTree.AuditHashPairs(node.Hash, proof);

            foreach (var pair in auditPairs)
            {
                DemoMerkleNode left = tree.RootNode.Cast<DemoMerkleNode>().First(t => t.Hash == pair.Item1);
                DemoMerkleNode right = tree.RootNode.Cast<DemoMerkleNode>().First(t => t.Hash == pair.Item2);
                tbAuditTrail.AppendText(left.Text + " + " + right.Text + " = " + left.Text + right.Text + CRLF);
            }
        }

        protected void DrawConsistencyProof(MerkleTree tree, DemoMerkleNode oldTreeRoot, int m, MerkleHash newRootHash)
        {
            tree.RootNode.Leaves().Cast<DemoMerkleNode>().Take(m).ForEach(n => Highlight(n, "Orange"));

            List<MerkleProofHash> proofToOldRoot = tree.ConsistencyProof(m);
            bool ret = MerkleTree.VerifyConsistency(oldTreeRoot.Hash, proofToOldRoot);
            lblConsistencyPassFail.Text = ret ? "Pass" : "Fail";
            lblConsistencyPassFail.ForeColor = ret ? Color.Green : Color.Red;

            tbConsistencyTrail.Clear();

            proofToOldRoot.ForEach(hash =>
            {
                var node = tree.RootNode.Cast<DemoMerkleNode>().First(t => t.Hash == hash.Hash);
                Highlight(node);
                tbConsistencyTrail.AppendText(node.Text + CRLF);
            });

            tbConsistencyTrail.AppendText(oldTreeRoot.Text + " = old root" + CRLF);

            if (!ckOnlyToOldRoot.Checked)
            {
                // The remainder: consistency audit proof.

                var lastNode = proofToOldRoot.Last();
                List<MerkleProofHash> proofToNewRoot = tree.ConsistencyAuditProof(lastNode.Hash);

                foreach (var auditHash in proofToNewRoot)
                {
                    // We use First because a tree with an odd number of leaves will duplicate the last leaf
                    // when computing the hash.
                    var n = tree.RootNode.Cast<DemoMerkleNode>().First(t => t.Hash == auditHash.Hash);
                    // tbAuditTrail.AppendText(n.Text + CRLF);

                    if (!proofToOldRoot.Any(ph => ph.Hash == n.Hash))
                    {
                        Highlight(n, "Purple");
                    }
                }

                var auditPairs = MerkleTree.AuditHashPairs(lastNode.Hash, proofToNewRoot);
                string lastHash = "";

                foreach (var pair in auditPairs)
                {
                    var left = tree.RootNode.Cast<DemoMerkleNode>().First(t => t.Hash == pair.Item1);
                    var right = tree.RootNode.Cast<DemoMerkleNode>().First(t => t.Hash == pair.Item2);
                    lastHash = left.Text + right.Text;
                    tbConsistencyTrail.AppendText(left.Text + " + " + right.Text + " = " + lastHash + CRLF);
                }

                tbConsistencyTrail.AppendText(lastHash + " = new root" + CRLF);
            }
        }

        protected List<Rectangle> DrawLeaves(List<MerkleNode> leaves)
        {
            List<Rectangle> rects = new List<Rectangle>();
            int i = 0;

            foreach (var leaf in leaves.Cast<DemoMerkleNode>())
            {
                var leafRect = new Rectangle(i * X_OFFSET, LEAF_Y, LEAF_WIDTH, NODE_HEIGHT);
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
            int indent;
            int spacing;
            int width = NODE_WIDTH;

            // Leaves and branches have different widths.
            if (level == 1)
            {
                indent = LEAF_WIDTH / 2 - 5;
                spacing = LEAF_WIDTH * 2 + 10;
            }
            else if (level == 2)
            {
                indent = ((int)Math.Pow(2, l0) - 1) * LEAF_WIDTH + LEAF_WIDTH / 2;
                spacing = NODE_WIDTH * (int)Math.Pow(2, level) - NODE_WIDTH / 2 - 13;
            }
            else if (level == 3)
            {
                indent = ((int)Math.Pow(2, l0) - 1) * LEAF_WIDTH + LEAF_WIDTH / 2 + 10;
                spacing = NODE_WIDTH * (int)Math.Pow(2, level) - NODE_WIDTH / 2 - 75;
            }
            else
            {
                indent = ((int)Math.Pow(2, l0) - 1) * LEAF_WIDTH + LEAF_WIDTH / 2 - 20;
                spacing = NODE_WIDTH * (int)Math.Pow(2, level) - NODE_WIDTH / 2 - 30;
                width = NODE_WIDTH * 2;
            }

            Color nodeColor = parents.Count() == 1 ? ROOT_COLOR : NODE_COLOR;

            foreach (var node in parents.Cast<DemoMerkleNode>())
            {
                var nodeRect = new Rectangle(indent + i * spacing, LEAF_Y - (V_OFFSET * level), width, NODE_HEIGHT);
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
                WebSocketHelpers.DropConnector("DynamicConnectorUD", "", bottomMiddle.X, bottomMiddle.Y, topMiddle.X, topMiddle.Y);
                n++;
                n2 = n2 + ((n % 2) == 0 ? 1 : 0);
            }
        }

        protected void Highlight(DemoMerkleNode node, string color = "Yellow")
        {
            Rectangle r = (Rectangle)node.Tag;
            WebSocketHelpers.UpdateProperty(node.Text, "FillColor", color);
        }

        private void NumLeavesChanged(object sender, EventArgs e)
        {
            WebSocketHelpers.ClearCanvas();
            MerkleTree tree = new DemoMerkleTree();
            CreateTree(tree, (int)nudNumLeaves.Value);
            DrawTree(tree);

            // Adjust maximums of our "proof" explorers.
            nudAuditProofNodeNumber.Maximum = nudNumLeaves.Value - 1;
            nudConsistencyProofNumLeaves.Maximum = nudNumLeaves.Value - 1;
        }

        private void btnAuditProof_Click(object sender, EventArgs e)
        {
            WebSocketHelpers.ClearCanvas();
            MerkleTree tree = new DemoMerkleTree();
            CreateTree(tree, (int)nudNumLeaves.Value);
            DrawTree(tree);
            int leafNum = (int)nudAuditProofNodeNumber.Value;
            Highlight(tree.RootNode.Leaves().Cast<DemoMerkleNode>().Single(n=>n.Text == leafNum.ToString("X")), "Orange");
            DrawAuditProof(leafNum.ToString("X"), tree);
        }

        private void btnConsistencyProof_Click(object sender, EventArgs e)
        {
            WebSocketHelpers.ClearCanvas();
            MerkleTree tree = new DemoMerkleTree();
            CreateTree(tree, (int)nudNumLeaves.Value);
            DrawTree(tree);
            int numLeaves = (int)nudConsistencyProofNumLeaves.Value;

            // Reconstruct the old root hash by creating a tree of "m" leaves.
            // We do this because in the demo, we haven't given the user the ability to append trees, so we
            // simulate that process here.
            MerkleTree oldTree = new DemoMerkleTree();
            CreateTree(oldTree, numLeaves);

            // For demo purposes, remove any () that were created by left-only branches.
            ((DemoMerkleNode)oldTree.RootNode).Text = ((DemoMerkleNode)oldTree.RootNode).Text.Replace("(", "").Replace(")", "");

            DrawConsistencyProof(tree, (DemoMerkleNode)oldTree.RootNode, numLeaves, null);
        }
    }
}



