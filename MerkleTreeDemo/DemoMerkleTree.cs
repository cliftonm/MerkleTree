using Clifton.Blockchain;

namespace MerkleTreeDemo
{
    public class DemoMerkleTree : MerkleTree
    {
        protected override MerkleNode CreateNode(MerkleHash hash)
        {
            return new DemoMerkleNode(hash);
        }

        protected override MerkleNode CreateNode(MerkleNode left, MerkleNode right)
        {
            return new DemoMerkleNode((DemoMerkleNode)left, (DemoMerkleNode)right);
        }
    }
}
