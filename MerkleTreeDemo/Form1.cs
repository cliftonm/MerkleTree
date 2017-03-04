using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using FlowSharpLib;

using Clifton.Blockchain;

namespace MerkleTreeDemo
{
    public partial class Form1 : Form
    {
        public const int LEAF_Y = 300;
        public const int V_OFFSET = 60;
        public const int X_OFFSET = 40;

        public Form1()
        {
            InitializeComponent();
            Test();
        }

        public void Test()
        {
            MerkleTree tree = new MerkleTree();

            for (int i = 0; i < 20; i++)
            {
                tree.AppendLeaf(MerkleHash.Create(i.ToString()));
            }

            tree.BuildTree();

            MerkleNode rootNode = tree.RootNode;

            // Get all leaves
            List<MerkleNode> leaves = new List<MerkleNode>();

            TraverseTree(rootNode, leaves);
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

        protected void TraverseTree(MerkleNode node, List<MerkleNode> leaves)
        {
            if (node.LeftNode == null && node.RightNode == null)
            {
                leaves.Add(node);
            }
            else
            {
                // Left node will always exist.
                TraverseTree(node.LeftNode, leaves);

                // Right node may not exist and end of leaves.
                if (node.RightNode != null)
                {
                    TraverseTree(node.RightNode, leaves);
                }
            }
        }

        protected List<Rectangle> DrawLeaves(List<MerkleNode> leaves)
        {
            List<Rectangle> rects = new List<Rectangle>();
            int i = 0;

            foreach (var leave in leaves)
            {
                WebSocketHelpers.DropShape("Box", i * X_OFFSET, LEAF_Y, 30, 30, i.ToString());
                rects.Add(new Rectangle(i * X_OFFSET, LEAF_Y, 30, 30));
                ++i;
            }

            return rects;
        }

        protected List<Rectangle> DrawParents(IEnumerable<MerkleNode> parents, int level)
        {
            List<Rectangle> rects = new List<Rectangle>();
            int i = 0;
            int l0 = level - 1;

            foreach (var node in parents)
            {               
                //                                       an = 2^n-1
                // 1 : .5            0 : .5           0: 0
                // 2 : 1.5           1 : 1.5          1: 1
                // 3 : 3.5           2 : 3.5          2: 3
                // 4 : 7.5           3 : 7.5          3: 7
                // deltas are 1, 2, 4, 8,....
                int xstart = ((int)Math.Pow(2, l0) - 1) * X_OFFSET + X_OFFSET / 2;
                WebSocketHelpers.DropShape("Box", xstart + i * X_OFFSET * (int)Math.Pow(2, level), LEAF_Y - (V_OFFSET * level), 30, 30, i.ToString());
                rects.Add(new Rectangle(xstart + i * X_OFFSET * (int)Math.Pow(2, level), LEAF_Y - (V_OFFSET * level), 30, 30));

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
                WebSocketHelpers.DropConnector("DynamicConnectorUD", bottomMiddle.X, bottomMiddle.Y, topMiddle.X, topMiddle.Y);
                n++;
                n2 = n2 + ((n % 2) == 0 ? 1 : 0);
            }
        }
    }
}



