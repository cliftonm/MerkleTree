using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Clifton.Blockchain
{
    public class MerkleHash
    {
        public byte[] Value { get; protected set; }

        protected MerkleHash()
        {
        }

        public static MerkleHash Create(byte[] buffer)
        {
            MerkleHash hash = new MerkleHash();
            hash.ComputeHash(buffer);

            return hash;
        }

        public static MerkleHash Create(string buffer)
        {
            return Create(Encoding.UTF8.GetBytes(buffer));
        }

        /// <summary>
        /// Compute the SHA256 hash of the data.
        /// </summary>
        public void ComputeHash(byte[] buffer)
        {
            SHA256 sha256 = SHA256Managed.Create();
            SetHash(sha256.ComputeHash(buffer));
        }

        public void SetHash(byte[] hash)
        {
            MerkleTree.Contract(() => hash.Length == Constants.HASH_LENGTH, "Unexpected hash length.");
            Value = hash;
        }

        public bool Equals(byte[] hash)
        {
            return Value.SequenceEqual(hash);
        }

        public bool Equals(MerkleHash hash)
        {
            return Value.SequenceEqual(hash.Value);
        }
    }
}
