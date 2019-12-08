using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace AVLTree.UnitTests
{
    public class BinarySearchSectionTests
    {
        public static IEnumerable<object[]> GetKeys()
        {
            yield return new object[] {new [] {1, 2, 3, 4, 5, 6}};
            yield return  new object[] {new [] {1, 2}};
            yield return  new object[] {new [] {5, 4, 3, 2, 1}};
            yield return  new object[] {new [] {2, 1, 3, 5, 4}};
            yield return  new object[] {new [] {2, 1}};
            yield return new object[] { new [] {1}};
        }

        

        [Theory]
        [MemberData(nameof(GetKeys))]
        public void IsBinarySearchTreeTest(int[] keys) //Test Insert Method
        {
            if (keys.Any())
            {
                var root = keys.CreateTree();
                var orderedByTree = root.InOrderSearch()
                    .Select(x => x.Key)
                    .ToArray();

                Assert.Equal(keys.OrderBy(x => x).ToArray(), orderedByTree);
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        [InlineData(1000)]
        [InlineData(10000)]
        public void BinaryFindMethodTest(int count)
        {
            var keys = Enumerable.Range(1, count).ToArray();
            if (keys.Any())
            {
                var root = keys.CreateTreeRotated();
                var answers = Enumerable.Range(0, count + 2)
                    .Select(x =>
                    {
                        var contains = root.TryFind(x, out TreeNode foundNode);
                        return new {contains, foundNode, key = x};
                    })
                    .ToArray();

                var expected = Enumerable.Repeat(false, 1)
                    .Concat(Enumerable.Range(1, count).Select(_ => true))
                    .Concat(Enumerable.Repeat(false, 1))
                    .ToArray();


                Assert.All(answers, x => Assert.True(x.contains ? x.foundNode.Key == x.key : x.foundNode == null));
                Assert.Equal(expected, answers.Select(x => x.contains));
            }
        }


        [Theory]
        [MemberData(nameof(GetKeys))]
        public void AllExceptRootHasParentTest(int[] keys)
        {
            if (keys.Any())
            {
                var root = keys.CreateTree();
                Assert.All(root.InOrderSearch(), node => Assert.True(node == root || node.Parent != null));
                Assert.All(root.InOrderSearch(),
                    node =>
                    {
                        if (node.GetChildDirection() == ChildDirection.Left)
                        {
                            Assert.True(node.Key < node.Parent.Key);
                        }

                        if (node.GetChildDirection() == ChildDirection.Right)
                        {
                            Assert.True(node.Key > node.Parent.Key);
                        }

                        if (node.Parent != null)
                        {
                            Assert.True(node.Parent.Height >= node.Height + 1);
                        }
                    });
                Assert.All(root.InOrderSearch(), node => Assert.Equal(node.CalcHeight(), node.Height));
            }
        }
    }
}
