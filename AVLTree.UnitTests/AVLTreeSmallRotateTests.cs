using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AVLTree.UnitTests
{
    public class AVLTreeSmallRotateTests
    {
        public class SmallRotateTestData
        {
            public int[] Keys;
            public int RotationNodeKey;
            public int HeightBeforeUpdate;
            public int HeightAfterUpdate;
            public ChildDirection direction;

            public SmallRotateTestData(int[] keys, int rotationNodeKey, int heightBeforeUpdate, int heightAfterUpdate, ChildDirection direction)
            {
                Keys = keys;
                RotationNodeKey = rotationNodeKey;
                HeightBeforeUpdate = heightBeforeUpdate;
                HeightAfterUpdate = heightAfterUpdate;
                this.direction = direction;
            }
        }

        /// <summary>
        /// SmallRotateData
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<object[]> GetTestData()
        {
            yield return new object[] { new SmallRotateTestData(new[] { 3, 2, 1 }, 3, 1, 1, ChildDirection.Right) };
            yield return new object[] { new SmallRotateTestData(new[] { 1, 2, 3 }, 1, 1, 1, ChildDirection.Left) };
            yield return new object[] { new SmallRotateTestData(new[] { 5, 3, 2, 4, 1, 6 }, 5, 2, 2, ChildDirection.Right) };
            yield return new object[] { new SmallRotateTestData(new[] { 2, 1, 4, 3, 5, 6 }, 2, 2, 2, ChildDirection.Left) };
            yield return new object[] { new SmallRotateTestData(new[] { 7, 5, 3, 2, 4, 1, 6 }, 5, 4, 3, ChildDirection.Right) };
            yield return new object[] { new SmallRotateTestData(new[] { 0, 5, 3, 2, 4, 1, 6 }, 5, 4, 3, ChildDirection.Right) };
            yield return new object[] { new SmallRotateTestData(new[] { 0, 2, 1, 4, 3, 5, 6 }, 2, 4, 3, ChildDirection.Left) };
            yield return new object[] { new SmallRotateTestData(new[] { 7, 2, 1, 4, 3, 5, 6 }, 2, 4, 3, ChildDirection.Left) };
            yield return new object[] { new SmallRotateTestData(new[] { 4, 3, 2, 1 }, 4, 2, 2, ChildDirection.Right) };
            yield return new object[] { new SmallRotateTestData(new[] { 4, 3, 2, 1 }, 3, 3, 2, ChildDirection.Right) };//Rotation doesn't change root heightBeforeUpdate

            // yield return new object[] {new [] {1, 2, 3, 4, 5, 6}};
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void SmallRotateTest(SmallRotateTestData smallRotateTestData)
        {
            int[] keys = smallRotateTestData.Keys;
            int rotationNodeKey = smallRotateTestData.RotationNodeKey;
            int heightBeforeUpdate = smallRotateTestData.HeightBeforeUpdate;
            int heightAfterUpdate = smallRotateTestData.HeightAfterUpdate;
            ChildDirection direction = smallRotateTestData.direction;

            if (keys.Any())
            {
                var root = keys.CreateTree();
                var nodes = root.InOrderSearch().ToArray();
                var rotationNode = nodes.FirstOrDefault(x => x.Key == rotationNodeKey);
                TreeNode newNode = null;
                if (direction == ChildDirection.Left)
                {
                    newNode = rotationNode.SmallLeftRotate(); //operation 
                }
                else if (direction == ChildDirection.Right)
                {
                    newNode = rotationNode.SmallRightRotate(); //operation
                }
                var newRoot = nodes.Single(x => x.Parent == null);

                Assert.True(nodes.All(IsLinkCorrect));
                Assert.Equal(keys.OrderBy(x => x).ToArray(), newRoot.InOrderSearch().Select(x => x.Key).ToArray());
                Assert.Equal(heightBeforeUpdate, newRoot.InOrderSearch().Max(x => x.Height));

                newNode.UpdateHeights();
                Assert.Equal(heightAfterUpdate, newRoot.InOrderSearch().Max(x => x.Height));
                Assert.All(nodes, node => Assert.Equal(node.CalcHeight(), node.Height));
            }
        }

        private static bool IsLinkCorrect(TreeNode treeNode)
        {
            if (treeNode.Left != null && treeNode.Left.Parent != treeNode)
            {
                return false;
            }

            if (treeNode.Right != null && treeNode.Right.Parent != treeNode)
            {
                return false;
            }
            return true;
        }
    }
}
