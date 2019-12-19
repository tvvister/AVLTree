using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("AVLTree.UnitTests")]
namespace AVLTree
{
    public sealed class TreeNode
    {
        public TreeNode Left = null;
        public TreeNode Right = null;
        public TreeNode Parent = null;
        public int Key;
        internal int Height;
        internal long ComposeValue;
        public TreeNode(int key)
        {
            Key = key;
            ComposeValue = key;
        }
    }
}
