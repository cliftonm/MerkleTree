using System;
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

        public static bool operator ==(MerkleHash h1, MerkleHash h2)
        {
            return h1.Equals(h2);
        }

        public static bool operator !=(MerkleHash h1, MerkleHash h2)
        {
            return !h1.Equals(h2);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            MerkleTree.Contract(() => obj is MerkleHash, "rvalue is not a MerkleHash");
            return Equals((MerkleHash)obj);
        }

        public override string ToString()
        {
            return BitConverter.ToString(Value).Replace("-", "");
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
            bool ret = false;

            if (((object)hash) != null)
            {
                ret = Value.SequenceEqual(hash.Value);
            }

            return ret;
        }
    }
}
