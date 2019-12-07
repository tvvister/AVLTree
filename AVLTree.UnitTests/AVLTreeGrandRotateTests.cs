using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AVLTree.UnitTests
{
    public class AVLTreeGrandRotateTests
    {
        public class GrandRotateTestData
        {
            public int[] Keys;
            public int RotationNodeKey;
            public int HeightAfterUpdate;
            public ChildDirection direction;

            public GrandRotateTestData(int[] keys, int rotationNodeKey, int heightAfterUpdate, ChildDirection direction)
            {
                Keys = keys;
                RotationNodeKey = rotationNodeKey;
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
            yield return new object[] { new GrandRotateTestData(new[] { 16,8,5,12,11,14,17 }, 16, 2, ChildDirection.Right) };
            yield return new object[] { new GrandRotateTestData(new[] { 19,16,8,5,12,11,14,17 }, 16, 3, ChildDirection.Right) };
            yield return new object[] { new GrandRotateTestData(new[] { 1,16,8,5,12,14,17 }, 16, 3, ChildDirection.Right) };
            yield return new object[] { new GrandRotateTestData(new[] { 1,16,8,5,12,11,17 }, 16, 3, ChildDirection.Right) };
            yield return new object[] { new GrandRotateTestData(new[] { 16, 8, 5, 12, 11, 14, 17 }, 16, 2, ChildDirection.Right) };

            yield return new object[] { new GrandRotateTestData(new[] { 16,15,20,18,25,17,19 }, 16, 2, ChildDirection.Left) };
            yield return new object[] { new GrandRotateTestData(new[] { 16,15,20,18,25,19 }, 16, 2, ChildDirection.Left) };
            yield return new object[] { new GrandRotateTestData(new[] { 16,15,20,18,25,17 }, 16, 2, ChildDirection.Left) };
            yield return new object[] { new GrandRotateTestData(new[] { 1,16,15,20,18,25,17,19 }, 16, 3, ChildDirection.Left) };
            yield return new object[] { new GrandRotateTestData(new[] { 30,16,15,20,18,25,17,19 }, 16, 3, ChildDirection.Left) };
            yield return new object[] { new GrandRotateTestData(new[] { 30,16,15,20,18,25,17,19,26 }, 16, 4, ChildDirection.Left) };
            //yield return new object[] { new GrandRotateTestData(new[] { 1, 2, 3 }, 1, 1, ChildDirection.Left) };


            // yield return new object[] {new [] {1, 2, 3, 4, 5, 6}};
        }

        [Theory]
        [MemberData(nameof(GetTestData))]
        public void GrandRotateTest(GrandRotateTestData grandRotateTestData)
        {
            int[] keys = grandRotateTestData.Keys;
            int rotationNodeKey = grandRotateTestData.RotationNodeKey;
            int heightAfterUpdate = grandRotateTestData.HeightAfterUpdate;
            ChildDirection direction = grandRotateTestData.direction;

            if (keys.Any())
            {
                var root = keys.CreateTree();
                var nodes = root.InOrderSearch().ToArray();
                var rotationNode = nodes.FirstOrDefault(x => x.Key == rotationNodeKey);
                TreeNode newNode = null;
                if (direction == ChildDirection.Left)
                {
                    newNode = rotationNode.GrandLeftRotate(); //operation 
                }
                else if (direction == ChildDirection.Right)
                {
                    newNode = rotationNode.GrandRightRotate(); //operation
                }
                var newRoot = nodes.Single(x => x.Parent == null);

                Assert.True(nodes.All(IsLinkCorrect));
                Assert.Equal(keys.OrderBy(x => x).ToArray(), newRoot.InOrderSearch().Select(x => x.Key).ToArray());
                
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
