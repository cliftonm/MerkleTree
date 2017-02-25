namespace Clifton.Blockchain
{
    public class MerkleAuditHash
    {
        public enum Branch
        {
            Left,
            Right,
        }

        public MerkleHash Hash { get; protected set; }
        public Branch Direction { get; protected set; }

        public MerkleAuditHash(MerkleHash hash, Branch direction)
        {
            Hash = hash;
            Direction = direction;
        }

        public override string ToString()
        {
            return Hash.ToString();
        }
    }
}
