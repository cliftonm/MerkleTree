using System.Linq;
using System.Security.Cryptography;

namespace Clifton.Blockchain
{
    public class MerkleNode
    {
        public MerkleHash Hash { get; protected set; }
        public MerkleNode LeftNode { get; protected set; }
        public MerkleNode RightNode { get; protected set; }
        public MerkleNode Parent { get; protected set; }

        public MerkleNode()
        {
        }

        /// <summary>
        /// Constructor for a base node, representing the lowest level of the tree.
        /// </summary>
        public MerkleNode(MerkleHash hash)
        {
            Hash = hash;
        }

        public override string ToString()
        {
            return Hash.ToString();
        }

        /// <summary>
        /// Constructor for a parent node.
        /// </summary>
        public MerkleNode(MerkleNode left, MerkleNode right = null)
        {
            LeftNode = left;
            RightNode = right;
            LeftNode.Parent = this;

            if (RightNode != null)
            {
                RightNode.Parent = this;
            }

            ComputeHash();
        }

        public void ComputeHash(byte[] buffer)
        {
            Hash = MerkleHash.Create(buffer);
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
            MerkleHash rightHash = RightNode == null ? LeftNode.Hash : RightNode.Hash;
            ComputeHash(LeftNode.Hash.Value.Concat(rightHash.Value).ToArray());
            Parent?.ComputeHash();      // Recurse, because out hash has changed.
        }
    }
}
