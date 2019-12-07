using System.Linq;

namespace AVLTree.UnitTests
{
    public static class TestUtils
    {
        public static TreeNode CreateTree(this int[] keys)
        {
            var root = new TreeNode(keys.First());
            foreach (var key in keys.Skip(1))
            {
                root.Insert(new TreeNode(key));
            }
            return root;
        }
        public static TreeNode CreateTreeRotated(this int[] keys)
        {
            var root = new TreeNode(keys.First());
            foreach (var key in keys.Skip(1))
            {
                root = root.InsertRotated(new TreeNode(key));
            }
            return root;
        }
    }
}
