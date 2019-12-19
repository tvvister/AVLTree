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

        public static TreeNode InsertRotated(this TreeNode node, TreeNode newNode, Action<TreeNode> composeValueSetter = null)
        {
            node.InsertInPlace(newNode);
            var rootOfChangedSubTree = UpdateHeightsRotated(newNode.Parent, composeValueSetter);
            return rootOfChangedSubTree.Parent == null ? rootOfChangedSubTree : node;
        }

        private static TreeNode UpdateHeightsRotated(this TreeNode node, Action<TreeNode> composeValueSetter = null)
        {
            var cur = node;
            while (cur != null)
            {
                var oldHeight = cur.Height;
                var oldCompValue = cur.ComposeValue;
                if (!cur.RotateIfNeed(out cur, composeValueSetter))
                {
                    cur.UpdateOwnHeight();
                    composeValueSetter?.Invoke(cur);
                }

                if (cur.Height == oldHeight && oldCompValue == cur.ComposeValue)
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

        private static bool RotateIfNeed(this TreeNode node, out TreeNode replacedNode, Action<TreeNode> composeValueSetter = null)
        {
            var delta = node.Left.GetHeight() - node.Right.GetHeight();
            if (Math.Abs(delta) == 2)
            {
                if (delta > 0)
                {
                    var leftHeight = node.Left.Left.GetHeight();
                    var rightHeight = node.Left.Right.GetHeight();
                    replacedNode = leftHeight >= rightHeight ? SmallRightRotate(node, composeValueSetter) : GrandRightRotate(node, composeValueSetter);
                }
                else
                {
                    var leftHeight = node.Right.Left.GetHeight();
                    var rightHeight = node.Right.Right.GetHeight();
                    replacedNode = leftHeight <= rightHeight ? SmallLeftRotate(node, composeValueSetter) : GrandLeftRotate(node, composeValueSetter);
                }
                return true;
            }
            replacedNode = node;
            return false;
        }
        internal static TreeNode SmallRightRotate(this TreeNode node, Action<TreeNode> composeValueSetter = null)
        {
            return node.SmallRotate(
                childProvider: x => x.Left,
                oppositeChildProvider: x => x.Right,
                childSetterAction: (x, c) => x.Right = c,
                anotherChildSetterAction: (x, c) => x.Left = c,
                composeValueSetter: composeValueSetter);
        }

        internal static TreeNode SmallLeftRotate(this TreeNode node, Action<TreeNode> composeValueSetter = null)
        {
            return node.SmallRotate(
                childProvider : x => x.Right,
                oppositeChildProvider : x => x.Left,
                childSetterAction : (x, c) => x.Left = c,
                anotherChildSetterAction : (x, c) => x.Right = c,
                composeValueSetter: composeValueSetter);
        }
        internal static TreeNode GrandLeftRotate(this TreeNode node, Action<TreeNode> composeValueSetter = null)
        {
            return node.GrandRotate(
                highGrandsonFunc: x => x.Right.Left,
                highSonFunc: x => x.Right,
                farMovingNodeFunc: x => x.Left,
                closelyMovingNodeFunc: x => x.Right, 
                composeValueSetter: composeValueSetter);
        }

        internal static TreeNode GrandRightRotate(this TreeNode node, Action<TreeNode> composeValueSetter = null)
        {
            return node.GrandRotate(
                highGrandsonFunc: x => x.Left.Right,
                highSonFunc: x => x.Left,
                farMovingNodeFunc: x => x.Right,
                closelyMovingNodeFunc: x => x.Left,
                composeValueSetter: composeValueSetter);
        }

        private static TreeNode GrandRotate(this TreeNode node, 
            Func<TreeNode, TreeNode> highSonFunc,
            Func<TreeNode, TreeNode> highGrandsonFunc,
            Func<TreeNode, TreeNode> farMovingNodeFunc,
            Func<TreeNode, TreeNode> closelyMovingNodeFunc,
            Action<TreeNode> composeValueSetter = null
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
            composeValueSetter?.Invoke(highSon);
            node.UpdateOwnHeight();
            composeValueSetter?.Invoke(node);
            highGrandson.UpdateOwnHeight();
            composeValueSetter?.Invoke(highGrandson);
            return highGrandson;
        }

        private static void UpdateParent(this TreeNode node, TreeNode parent)
        {
            if (node != null)
            {
                node.Parent = parent;
            }
        }

        private static void ReplaceChild(this TreeNode parent, TreeNode oldChild, TreeNode newChild)
        {
            if (parent == null)
            {
                newChild.UpdateParent(null);
                return;
            }
            if (parent.Left == null && parent.Right == null)
            {
                if (oldChild != null) throw new ArgumentException($"{oldChild} must be null if parent children are null");
                if (newChild != null)
                {
                    if (newChild.Key < parent.Key)
                    {
                        parent.Left = newChild;
                    }
                    else if (newChild.Key > parent.Key)
                    {
                        parent.Right = newChild;
                    }
                    newChild.Parent = parent;
                }
            }
            else if (parent.Left != null && parent.Right != null)
            {
                if (oldChild == parent.Right)
                {
                    parent.Right = newChild;
                }
                else if (oldChild == parent.Left)
                {
                    parent.Left = newChild;
                }
                newChild.UpdateParent(parent);
            }
            else if (parent.Left == null)
            {
                if (parent.Right == oldChild)
                {
                    parent.Right = newChild;
                }
                else
                {
                    parent.Left = newChild;
                }
                newChild.UpdateParent(parent);
            }
            else if (parent.Right == null)
            {
                if (parent.Left == oldChild)
                {
                    parent.Left = newChild;
                }
                else
                {
                    parent.Right = newChild;
                }
                newChild.UpdateParent(parent);
            }
        }

        ///returns new Root of subtree with previous root = argument node
        private static TreeNode SmallRotate(this TreeNode node, 
            Func<TreeNode, TreeNode> childProvider,
            Func<TreeNode, TreeNode> oppositeChildProvider,
            Action<TreeNode, TreeNode> childSetterAction,
            Action<TreeNode, TreeNode> anotherChildSetterAction,
            Action<TreeNode> composeValueSetter = null
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
            composeValueSetter?.Invoke(node);
            son.UpdateOwnHeight();
            composeValueSetter?.Invoke(son);
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

        internal static long CalcSum(this TreeNode node)
        {
            return Enumerable.Repeat((long)node.Key, 1)
                .Append(node.Left?.CalcSum() ?? 0)
                .Append(node.Right?.CalcSum() ?? 0)
                .Sum();
        }

        public static long CalcSumLess(this TreeNode node, long key)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.Key < key)
            {
                return node.Key + (node.Left?.ComposeValue ?? 0) + (node.Right?.CalcSumLess(key) ?? 0);
            }
            if (node.Key == key)
            {
                return (node.Left?.ComposeValue ?? 0);
            }
            return node.Left?.CalcSumLess(key) ?? 0;
        }

        public static long CalcSumGreater(this TreeNode node, long key)
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.Key > key)
            {
                return node.Key + (node.Right?.ComposeValue ?? 0) + (node.Left?.CalcSumGreater(key) ?? 0);
            }
            if (node.Key == key)
            {
                return (node.Right?.ComposeValue ?? 0);
            }
            return node.Right?.CalcSumGreater(key) ?? 0;
        }

        internal static TreeNode GetMin(this TreeNode node)
        {
            while (node.Left != null)
            {
                node = node.Left;
            }
            return node;
        }


        public static TreeNode RemoveRotated(this TreeNode root, int key, Action<TreeNode> composeValueSetter = null)
        {
            var deepestAffectedNode = RemoveInPlace(root, key);
            if (deepestAffectedNode == null)
            {
                return null;
            }
            var rootOfChangedSubTree = UpdateHeightsRotated(deepestAffectedNode, composeValueSetter);
            return rootOfChangedSubTree.Parent == null ? rootOfChangedSubTree : root;
        }

        /// <summary>
        /// Returns deepest affected Node. Returns null if last node was removed
        /// </summary>
        /// <param name="root"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        internal static TreeNode RemoveInPlace(this TreeNode root, int key)
        {
            TreeNode treeNode = null;
            if (root.TryFind(key, out treeNode))
            {
                if (treeNode.Left == null && treeNode.Right == null)
                {
                    var childDirection = treeNode.GetChildDirection();
                    if (childDirection == ChildDirection.Left)
                    {
                        treeNode.Parent.Left = null;
                        return treeNode.Parent;
                    }

                    if (childDirection == ChildDirection.Right)
                    {
                        treeNode.Parent.Right = null;
                        return treeNode.Parent;
                    }
                    else
                    {
                        return null;//There are no children and no parent 
                    }
                }
                else if (treeNode.Left != null && treeNode.Right != null)
                {
                    if (treeNode.Right.Left == null)
                    {
                        treeNode.Key = treeNode.Right.Key;
                        var newChild = treeNode.Right.Right;
                        treeNode.Right = newChild;
                        if (newChild != null)
                        {
                            newChild.Parent = treeNode;
                        }
                        return treeNode;
                    }
                    else
                    {
                        var switchNode = treeNode.Right.GetMin();
                        treeNode.Key = switchNode.Key;
                        return RemoveInPlace(switchNode, switchNode.Key);
                    }
                }
                SkipSoloChild(treeNode.Left ?? treeNode.Right, treeNode);
                return treeNode;
            }
            return root;
        }

        internal static TreeNode Add(this TreeNode root, int key)
        {
            if (root == null) return new TreeNode(key);
            return root.InsertRotated(new TreeNode(key));
        }

        private static void SkipSoloChild(TreeNode switchNode, TreeNode treeNode)
        {
            if (switchNode == null) throw new ArgumentNullException(nameof(switchNode));
            if (treeNode == null) throw new ArgumentNullException(nameof(treeNode));
            if (treeNode.Right != switchNode && treeNode.Left != switchNode) throw new InvalidOperationException($"{nameof(switchNode)} must child of {nameof(treeNode)}");

            var rChild = switchNode.Right;
            var lChild = switchNode.Left;
            treeNode.Key = switchNode.Key;
            treeNode.Right = rChild;
            if (rChild != null)
            {
                rChild.Parent = treeNode;
            }

            treeNode.Left = lChild;
            if (lChild != null)
            {
                lChild.Parent = treeNode;
            }
        }
    }
}