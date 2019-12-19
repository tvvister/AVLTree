using System;
using System.Collections.Generic;
using System.Linq;

namespace AVLTree.UnitTests
{
    public static class TestUtils
    {
        private static Action<TreeNode> _composeValueSetter = node =>
        {
            node.ComposeValue = node.Key
                                + (node.Left?.ComposeValue ?? 0)
                                + (node.Right?.ComposeValue ?? 0);
        };
        public static TreeNode CreateRootNode(int key)
        {
            return new TreeNode(key);
        }

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

        public static TreeNode CreateTreeRotatedSumCalculated(this int[] keys)
        {
            var root = new TreeNode(keys.First());
            foreach (var key in keys.Skip(1))
            {
                root = root.InsertRotated(new TreeNode(key), _composeValueSetter);
            }
            return root;
        }

        public static TreeNode CreateTreeRotatedSumCalculatedWithRemoveOdd(this int[] keys)
        {
            var root = new TreeNode(keys.First());
            foreach (var key in keys.Skip(1))
            {
                root = root.InsertRotated(new TreeNode(key), _composeValueSetter);
            }

            foreach (var key in keys)
            {
                if (key % 2 == 1)
                {
                    root = root.RemoveRotated(key, _composeValueSetter);
                }
            }
            return root;
        }

    }
}
