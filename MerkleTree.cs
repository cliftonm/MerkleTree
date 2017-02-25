using System;
using System.Collections.Generic;

namespace Clifton.Blockchain
{
    public class MerkleTree
    {
        public MerkleNode RootNode { get; protected set; }

        protected List<MerkleNode> nodes;

        public static void Contract(Func<bool> action, string msg)
        {
            if (!action())
            {
                throw new MerkleException(msg);
            }
        }

        public MerkleTree()
        {
            nodes = new List<MerkleNode>();
        }

        public void AppendNode(MerkleHash hash)
        {
            nodes.Add(new MerkleNode(hash));
        }

        public void BuildTree()
        {
            BuildTree(nodes);
        }

        /// <summary>
        /// Reduce the current list of n nodes to n/2 parents.
        /// </summary>
        /// <param name="nodes"></param>
        protected void BuildTree(List<MerkleNode> nodes)
        {
            if (nodes.Count == 1)
            {
                RootNode = nodes[0];
            }
            else
            {
                List<MerkleNode> parents = new List<MerkleNode>();

                for (int i = 0; i < nodes.Count; i += 2)
                {
                    MerkleNode right = (i + 1 < nodes.Count) ? nodes[i + 1] : null;
                    MerkleNode parent = new MerkleNode(nodes[i], right);
                    parents.Add(parent);
                }

                BuildTree(parents);
            }
        }
    }
}
