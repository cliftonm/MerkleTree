/* 
* Copyright (c) Marc Clifton
* The Code Project Open License (CPOL) 1.02
* http://www.codeproject.com/info/cpol10.aspx
*/

using Clifton.Blockchain;
using Clifton.Core.ExtensionMethods;

namespace MerkleTreeDemo
{
    public class DemoMerkleNode : MerkleNode
    {
        public string Text { get; set; }     // Useful for diagramming.
        public object Tag { get; set; }      // Useful for diagramming.

        public DemoMerkleNode(DemoMerkleNode left, DemoMerkleNode right = null) : base(left, right)
        {
            MergeText(left, right);
        }

        public DemoMerkleNode(MerkleHash hash)
        {
            Hash = hash;
        }

        public override string ToString()
        {
            // Useful for debugging, we use the node text if it exists, otherwise return the hash as a string.
            return Text ?? Hash.ToString();
        }

        public static DemoMerkleNode Create(string s)
        {
            return new DemoMerkleNode(MerkleHash.Create(s));
        }

        public MerkleNode SetText(string text)
        {
            Text = text;

            return this;
        }

        public MerkleNode SetTag(object tag)
        {
            Tag = tag;

            return this;
        }

        protected void MergeText(MerkleNode left, MerkleNode right)
        {
            // Useful for debugging, we combine the text of the two nodes.
            string text = (((DemoMerkleNode)left)?.Text ?? "") + (((DemoMerkleNode)right)?.Text ?? "");

            if (!string.IsNullOrEmpty(text))
            {
                if (right == null)
                {
                    text = text.Parens();
                }

                Text = text;
            }
        }
    }
}
