using System;
using System.Linq;
using AVLTree;

namespace MainApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var keys = Enumerable.Range(1, 50000000).ToArray();
            if (keys.Any())
            {
                var root = CreateTreeRotated(keys);
            }

            //Console.ReadLine();
        }
        public static TreeNode CreateTreeRotated(int[] keys)
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
