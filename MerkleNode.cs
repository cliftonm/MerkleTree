using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Clifton.Blockchain
{
    public class MerkleNode : IEnumerable<MerkleNode>
    {
        public string Text { get; set; }     // Useful for diagramming.
        public object Tag { get; set; }      // Useful for diagramming.

        public MerkleHash Hash { get; protected set; }
        public MerkleNode LeftNode { get; protected set; }
        public MerkleNode RightNode { get; protected set; }
        public MerkleNode Parent { get; protected set; }

        public MerkleNode()
        {
        }

        /// <summary>
        /// Constructor for a base node (leaf), representing the lowest level of the tree.
        /// </summary>
        public MerkleNode(MerkleHash hash)
        {
            Hash = hash;
        }

        /// <summary>                
        /// Constructor for a parent node.
        /// </summary>
        public MerkleNode(MerkleNode left, MerkleNode right = null)
        {
            LeftNode = left;
            RightNode = right;
            LeftNode.Parent = this;
            MergeText(left, right);
            ComputeHash();
        }

        public override string ToString()
        {
            // Useful for debugging, we use the node text if it exists, otherwise return the hash as a string.
            return Text ?? Hash.ToString();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<MerkleNode> GetEnumerator()
        {
            foreach (var n in Iterate(this)) yield return n;
        }

        protected IEnumerable<MerkleNode> Iterate(MerkleNode node)
        {
            if (node.LeftNode != null)
            {
                foreach (var n in Iterate(node.LeftNode)) yield return n;
            }

            if (node.RightNode != null)
            {
                foreach (var n in Iterate(node.RightNode)) yield return n;
            }

            yield return node;
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

        public MerkleHash ComputeHash(byte[] buffer)
        {
            Hash = MerkleHash.Create(buffer);

            return Hash;
        }

        /// <summary>
        /// Return the leaves (not all children, just leaves) under this node
        /// </summary>
        public IEnumerable<MerkleNode> Leaves()
        {
            return this.Where(n => n.LeftNode == null && n.RightNode == null);
        }

        public void SetLeftNode(MerkleNode node)
        {
            MerkleTree.Contract(() => node.Hash != null, "Node hash must be initialized.");
            LeftNode = node;
            LeftNode.Parent = this;
            ComputeHash();
        }

        public void SetRightNode(MerkleNode node)
        {
            MerkleTree.Contract(() => node.Hash != null, "Node hash must be initialized.");
            RightNode = node;
            RightNode.Parent = this;

            // Can't compute hash if the left node isn't set yet.
            if (LeftNode != null)
            {
                ComputeHash();
            }
        }

        /// <summary>
        /// True if we have enough data to verify our hash, particularly if we have child nodes.
        /// </summary>
        /// <returns>True if this node is a leaf or a branch with at least a left node.</returns>
        public bool CanVerifyHash()
        {
            return (LeftNode == null && RightNode == null) || (LeftNode != null);
        }

        /// <summary>
        /// Verifies the hash for this node against the computed hash for our child nodes.
        /// If we don't have any children, the return is always true because we have nothing to verify against.
        /// </summary>
        public bool VerifyHash()
        {
            if (LeftNode == null  && RightNode == null)
            {
                return true;
            }

            MerkleTree.Contract(() => LeftNode != null, "At least left node must be assigned.");
            SHA256 sha256 = SHA256Managed.Create();
            MerkleHash rightHash = RightNode == null ? LeftNode.Hash : RightNode.Hash;
            byte[] hash = sha256.ComputeHash(LeftNode.Hash.Value.Concat(rightHash.Value).ToArray());

            return Hash.Equals(hash);
        }

        /// <summary>
        /// If the hashes are equal, we know the entire node tree is equal.
        /// </summary>
        public bool Equals(MerkleNode node)
        {
            return Hash.Equals(node.Hash);
        }

        protected void ComputeHash()
        {
            // Repeat the left node if the right node doesn't exist.
            // This process breaks the case of doing a consistency check on 3 leaves when there are only 3 leaves in the tree.
            //MerkleHash rightHash = RightNode == null ? LeftNode.Hash : RightNode.Hash;
            //Hash = MerkleHash.Create(LeftNode.Hash.Value.Concat(rightHash.Value).ToArray());

            // Alternativately, do not repeat the left node, but carry the left node's hash up.
            // This process does not break the edge case described above.
            Hash = RightNode == null ? LeftNode.Hash : MerkleHash.Create(LeftNode.Hash.Value.Concat(RightNode.Hash.Value).ToArray());
            Parent?.ComputeHash();      // Recurse, because out hash has changed.
        }

        protected void MergeText(MerkleNode left, MerkleNode right)
        {
            Text = left.Text;

            if (RightNode != null)
            {
                RightNode.Parent = this;

                // Useful for debugging, we combine the text of the two nodes.
                if (Text != null)
                {
                    Text += right.Text ?? "";
                }
            }
        }
    }
}
