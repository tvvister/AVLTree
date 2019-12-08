using System;
using System.Collections.Generic;
using System.Linq;

namespace AVLTree
{
    public struct FindResult
    {
        public readonly TreeNode Parent;
        public readonly ChildDirection ChildDirection;

        public FindResult(TreeNode parent, ChildDirection childDirection)
        {
            Parent = parent;
            ChildDirection = childDirection;
        }
    }
    public static class TreeNodeExt
    {
        public static ChildDirection? GetChildDirection(this TreeNode node)
        {
            var parent = node?.Parent;
            if (parent == null) return null;
            return parent.Left == node ? ChildDirection.Left : ChildDirection.Right;
        }

        public static IEnumerable<TreeNode> InOrderSearch(this TreeNode node)
        {
            var inOrderSearch = Enumerable.Empty<TreeNode>();
            if (node == null)
            {
                return inOrderSearch;
            }
            return node.Left.InOrderSearch().Append(node).Concat(node.Right.InOrderSearch());
        }

        private static FindResult FindParent(this TreeNode root, TreeNode newNode)
        {
            var node = root;
            var isLess = newNode.Key < node.Key;
            var nextNode = isLess ? node.Left : node.Right;
            while (nextNode != null)
            {
                node = nextNode;
                isLess = newNode.Key < node.Key;
                nextNode = isLess ? node.Left : node.Right;
            }
            return new FindResult(node, isLess ? ChildDirection.Left : ChildDirection.Right);
        }

        public static bool TryFind(this TreeNode root, int key, out TreeNode foundNode)
        {

            var node = root;
            var nextNode = GetNextNode(node, key);
            while (nextNode != null && node.Key != key)
            {
                node = nextNode;
                nextNode = GetNextNode(node, key);
            }
            if (node.Key == key)
            {
                foundNode = node;
                return true;
            }
            foundNode = null;
            return false;
        }

        private static TreeNode GetNextNode(TreeNode node, int key)
        {
            var isLess = key < node.Key;
            return isLess ? node.Left : node.Right;
        }

        private static void InsertInPlace(this TreeNode node, TreeNode newNode)
        {
            var findResult = node.FindParent(newNode);
            if (findResult.ChildDirection == ChildDirection.Left)
            {
                findResult.Parent.Left = newNode;
            }
            else if (findResult.ChildDirection == ChildDirection.Right)
            {
                findResult.Parent.Right = newNode;
            }
            newNode.Parent = findResult.Parent;
        }
        public static void Insert(this TreeNode node, TreeNode newNode)
        {
            node.InsertInPlace(newNode: newNode);
            newNode.UpdateHeights();
        }

        public static TreeNode InsertRotated(this TreeNode node, TreeNode newNode)
        {
            node.InsertInPlace(newNode);
            var rootOfChangedSubTree = UpdateHeightsRotated(newNode.Parent);
            return rootOfChangedSubTree.Parent == null ? rootOfChangedSubTree : node;
        }

        private static TreeNode UpdateHeightsRotated(this TreeNode node)
        {
            var cur = node;
            while (cur != null)
            {
                var oldHeight = cur.Height;
                if (!cur.RotateIfNeed(out cur))
                {
                    cur.UpdateOwnHeight();
                }

                if (cur.Height == oldHeight)
                {
                    return cur;
                }
                if (cur.Parent == null)
                {
                    return cur;
                }
                cur = cur.Parent;
            }
            throw new NotImplementedException("Something wrong happened");
        }

        private static int GetNodeHeightByChildren(this TreeNode node)
        {
            if (node == null) return -1;
            if (node.Left == null && node.Right == null)
            {
                return 0;
            }

            if (node.Left == null)
            {
                return node.Right.Height + 1;
            }

            if (node.Right == null)
            {
                return node.Left.Height + 1;
            }

            return Math.Max(node.Left.Height, node.Right.Height) + 1;
        }

        private static int GetHeight(this TreeNode treeNode)
        {
            return treeNode?.Height ?? -1;
        }

        private static bool RotateIfNeed(this TreeNode node, out TreeNode replacedNode)
        {
            var delta = node.Left.GetHeight() - node.Right.GetHeight();
            if (Math.Abs(delta) == 2)
            {
                if (delta > 0)
                {
                    var leftHeight = node.Left.Left.GetHeight();
                    var rightHeight = node.Left.Right.GetHeight();
                    replacedNode = leftHeight >= rightHeight ? SmallRightRotate(node) : GrandRightRotate(node);
                }
                else
                {
                    var leftHeight = node.Right.Left.GetHeight();
                    var rightHeight = node.Right.Right.GetHeight();
                    replacedNode = leftHeight <= rightHeight ? SmallLeftRotate(node) : GrandLeftRotate(node);
                }
                return true;
            }
            replacedNode = node;
            return false;
        }
        internal static TreeNode SmallRightRotate(this TreeNode node)
        {
            return node.SmallRotate(
                childProvider: x => x.Left,
                oppositeChildProvider: x => x.Right,
                childSetterAction: (x, c) => x.Right = c,
                anotherChildSetterAction: (x, c) => x.Left = c);
        }

        internal static TreeNode SmallLeftRotate(this TreeNode node)
        {
            return node.SmallRotate(
                childProvider : x => x.Right,
                oppositeChildProvider : x => x.Left,
                childSetterAction : (x, c) => x.Left = c,
                anotherChildSetterAction : (x, c) => x.Right = c);
        }
        internal static TreeNode GrandLeftRotate(this TreeNode node)
        {
            return node.GrandRotate(
                highGrandsonFunc: x => x.Right.Left,
                highSonFunc: x => x.Right,
                farMovingNodeFunc: x => x.Left,
                closelyMovingNodeFunc: x => x.Right);
        }

        internal static TreeNode GrandRightRotate(this TreeNode node)
        {
            return node.GrandRotate(
                highGrandsonFunc: x => x.Left.Right,
                highSonFunc: x => x.Left,
                farMovingNodeFunc: x => x.Right,
                closelyMovingNodeFunc: x => x.Left);
        }

        private static TreeNode GrandRotate(this TreeNode node, 
            Func<TreeNode, TreeNode> highSonFunc,
            Func<TreeNode, TreeNode> highGrandsonFunc,
            Func<TreeNode, TreeNode> farMovingNodeFunc,
            Func<TreeNode, TreeNode> closelyMovingNodeFunc
            )
        {
            var parent = node.Parent;
            var highSon = highSonFunc(node);
            var highGrandson = highGrandsonFunc(node);
            var farMovingNode = farMovingNodeFunc(highGrandson);
            var closelyMovingNode = closelyMovingNodeFunc(highGrandson);

            highSon.ReplaceChild(highGrandson, closelyMovingNode);
            node.ReplaceChild(highSon, farMovingNode);

            parent.ReplaceChild(node, highGrandson);
            highGrandson.ReplaceChild(closelyMovingNode, highSon);
            highGrandson.ReplaceChild(farMovingNode, node);
            highSon.UpdateOwnHeight();
            node.UpdateOwnHeight();
            highGrandson.UpdateOwnHeight();

            return highGrandson;
        }
        
        private static void ReplaceChild(this TreeNode parent, TreeNode oldChild, TreeNode newChild)
        {
            if (parent != null && oldChild == parent.Right)
            {
                parent.Right = newChild;
            }
            else if (parent != null && oldChild == parent.Left)
            {
                parent.Left = newChild;
            }
            if (newChild != null)
            {
                newChild.Parent = parent;
            }
        }

        ///returns new Root of subtree with previous root = argument node
        private static TreeNode SmallRotate(this TreeNode node, 
            Func<TreeNode, TreeNode> childProvider,
            Func<TreeNode, TreeNode> oppositeChildProvider,
            Action<TreeNode, TreeNode> childSetterAction,
            Action<TreeNode, TreeNode> anotherChildSetterAction
        )
        {
            var parent = node.Parent;
            var son = childProvider(node);
            var grSon = oppositeChildProvider(son);

            var sonDirection = node.GetChildDirection();
            if (sonDirection == ChildDirection.Left) { parent.Left = son; }
            if (sonDirection == ChildDirection.Right) { parent.Right = son; }

            son.Parent = parent;

            childSetterAction(son, node);
            node.Parent = son;

            anotherChildSetterAction(node, grSon);
            if (grSon != null)
            {
                grSon.Parent = node;
                //grSon.UpdateOwnHeight();
            }

            node.UpdateOwnHeight();
            son.UpdateOwnHeight();
            return son;
        }

        internal static void UpdateHeights(this TreeNode node)
        {
            node.UpdateOwnHeight();
            var cur = node;
            while (cur.Parent != null && cur.Parent.Height != cur.Parent.GetNodeHeightByChildren())
            {
                cur.Parent.UpdateOwnHeight();
                cur = cur.Parent;
            }
        }

        internal static void UpdateOwnHeight(this TreeNode node)
        {
            if (node != null)
            {
                node.Height = node.GetNodeHeightByChildren();
            }
        }

        internal static int CalcHeight(this TreeNode node)
        {
            return Enumerable.Empty<TreeNode>()
                .Append(node.Left)
                .Append(node.Right)
                .Where(x => x != null)
                .Select(x => x.CalcHeight() + 1)
                .Append(0)
                .Max();
        }
    }
}