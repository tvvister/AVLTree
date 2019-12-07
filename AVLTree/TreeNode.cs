using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AVLTree.UnitTests")]
namespace AVLTree
{
    public sealed class TreeNode
    {
        public TreeNode Left = null;
        public TreeNode Right = null;
        public TreeNode Parent = null;
        public readonly int Key;
        internal int Height;

        public TreeNode(int key)
        {
            Key = key;
        }
    }
}
