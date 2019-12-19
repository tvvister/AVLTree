using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Xunit;

namespace AVLTree.UnitTests
{
    public class AVLTreeInsertRotatedTests
    {
        [Theory]
        [InlineData(10)]
        [InlineData(100)]
        [InlineData(200)]
        [InlineData(1000)]
        [InlineData(10000)]
        [InlineData(5000)]
        [InlineData(2000)]
        [InlineData(3000)]
        [InlineData(100000)]
        [InlineData(500000)]
        [InlineData(1000000)]
        [InlineData(2000000)]
        [InlineData(10000000)]
        //[InlineData(4000)]
        public void InsertRotatedBenchmarkTest(int count)
        {
            var keys = Enumerable.Range(1, count).ToArray();
            if (keys.Any())
            {
                var stopwatch = Stopwatch.StartNew();
                var root = keys.CreateTreeRotated();
                stopwatch.Stop();
                var AVLElapsed = stopwatch.Elapsed;
                stopwatch = Stopwatch.StartNew();
                var sortedDict = CreateSortedDict(keys);
                stopwatch.Stop();
                var RBElapsed = stopwatch.Elapsed;
                var nodes = root.InOrderSearch().ToArray();
                Assert.True(false, new { count, AVLElapsed, RBElapsed }.ToString());
            }
        }

        private static SortedDictionary<int, int> CreateSortedDict(int[] keys)
        {
            var sortedDict = new SortedDictionary<int, int>();
            foreach (var key in keys)
            {
                sortedDict.Add(key, key);
            }
            return sortedDict;
        }


        [Theory]
        //[InlineData(1)] Removing single node
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(100)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(500000)]
        public void RemoveRotatedTest(int count)
        {
            var keys = Enumerable.Range(1, count).ToArray();
            if (keys.Any())
            {
                var root = keys.CreateTreeRotated();
                
                foreach (var key in Enumerable.Range(1, count).Where(x => x % 2 == 1))
                {
                    root = root.RemoveRotated(key);
                }
                var nodes = root.InOrderSearch().ToArray();
                var newRoot = nodes.Single(x => x.Parent == null);

                Assert.True(nodes.All(IsLinkCorrect));
                Assert.Equal(Enumerable.Range(1, count).Where(x => x % 2 == 0).OrderBy(x => x), newRoot.InOrderSearch().Select(x => x.Key).ToArray());

                var maxHeight = nodes.Max(x => x.Height);
                Assert.InRange(maxHeight, 0, Math.Floor(1.45 * Math.Log(keys.Length + 2, 2)));
                Assert.All(nodes, node => Assert.Equal(node.CalcHeight(), node.Height));
            }
        }

        [Theory]
        //[InlineData(1)] Removing single node
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        [InlineData(6)]
        [InlineData(100)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(500000)]
        [InlineData(1000000)]
        public void RemoveRotatedSumCalculatingTest(int count)
        {
            var keys = Enumerable.Range(1, count).ToArray();
            if (keys.Any())
            {
                var root = keys.CreateTreeRotatedSumCalculatedWithRemoveOdd();
                var nodes = root.InOrderSearch().ToArray();
                var newRoot = nodes.Single(x => x.Parent == null);

                Assert.Equal(newRoot, root);
                Assert.True(nodes.All(IsLinkCorrect));
                Assert.Equal(Enumerable.Range(1, count).Where(x => x % 2 == 0).OrderBy(x => x), newRoot.InOrderSearch().Select(x => x.Key).ToArray());

                var maxHeight = nodes.Max(x => x.Height);
                Assert.InRange(maxHeight, 0, Math.Floor(1.45 * Math.Log(keys.Length + 2, 2)));
                Assert.All(nodes, node => Assert.Equal(node.CalcHeight(), node.Height));
                foreach (var treeNode in nodes)
                {
                    Assert.Equal(treeNode.CalcSum(), treeNode.ComposeValue);
                }
            }
        }



        [Fact]
        public void RemoveSingleNodeTest()
        {
            var root = TestUtils.CreateRootNode(1);
            root = root.RemoveRotated(1);
            Assert.Null(root);
        }

        [Fact]
        public void ZigZagTreeInsertRotatedTest()
        {
            var keys = new []{7,8,6,3,4};
            var root = keys.CreateTreeRotated();
            Assert.Equal(keys.OrderBy(x => x), root.InOrderSearch().Select(x => x.Key).ToArray());
        }

        [Theory]
        [InlineData(100)]
        [InlineData(10000)]
        [InlineData(100000)]
        [InlineData(500000)]
        public void InsertRotatedStressTest(int count)
        {
            var keys = Enumerable.Range(1, count).ToArray();
            if (keys.Any())
            {
                var root = keys.CreateTreeRotated();
                var nodes = root.InOrderSearch().ToArray();

                var newRoot = nodes.Single(x => x.Parent == null);

                Assert.True(nodes.All(IsLinkCorrect));
                Assert.Equal(keys.OrderBy(x => x).ToArray(), newRoot.InOrderSearch().Select(x => x.Key).ToArray());

                var maxHeight = nodes.Max(x => x.Height);
                Assert.InRange(maxHeight, 0, Math.Floor(1.45 * Math.Log(keys.Length + 2, 2)));
                Assert.All(nodes, node => Assert.Equal(node.CalcHeight(), node.Height));
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(10000)]
        [InlineData(100000)]
        //[InlineData(500000)]
        public void InsertRotatedSumCalculatingTest(int count)
        {
            var keys = Enumerable.Range(1, count).ToArray();
            if (keys.Any())
            {
                var root = keys.CreateTreeRotatedSumCalculated();
                var nodes = root.InOrderSearch().ToArray();

                var newRoot = nodes.Single(x => x.Parent == null);

                Assert.True(nodes.All(IsLinkCorrect));
                Assert.Equal(keys.OrderBy(x => x).ToArray(), newRoot.InOrderSearch().Select(x => x.Key).ToArray());

                var maxHeight = nodes.Max(x => x.Height);
                Assert.InRange(maxHeight, 0, Math.Floor(1.45 * Math.Log(keys.Length + 2, 2)));
                Assert.All(nodes, node => Assert.Equal(node.CalcHeight(), node.Height));

                //Assert.All(nodes, node => Assert.Equal(node.CalcSum(), node.ComposeValue));
                foreach (var treeNode in nodes)
                {
                    Assert.Equal(treeNode.CalcSum(), treeNode.ComposeValue);
                }
            }
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(10000)]
        [InlineData(100000)]
        //[InlineData(500000)]
        public void InsertRotatedSumOfLessSetTest(int count)
        {
            var keys = Enumerable.Range(1, count).ToArray();
            if (keys.Any())
            {
                var root = keys.CreateTreeRotatedSumCalculated();
                var nodes = root.InOrderSearch().ToArray();

                var newRoot = nodes.Single(x => x.Parent == null);
                //Assert.All(nodes, node => Assert.Equal(node.CalcSum(), node.ComposeValue));
                foreach (long n in keys.Skip(1))
                {
                    Assert.Equal(n*(n+1)/2, newRoot.CalcSumLess(n + 1));
                }
            }
        }
        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(100)]
        [InlineData(5000)]
        [InlineData(10000)]
        [InlineData(100000)]
        //[InlineData(500000)]
        public void InsertRotatedSumOfGreateSetTest(int count)
        {
            var keys = Enumerable.Range(1, count).ToArray();
            if (keys.Any())
            {
                var root = keys.CreateTreeRotatedSumCalculated();
                var nodes = root.InOrderSearch().ToArray();

                var newRoot = nodes.Single(x => x.Parent == null);
                //Assert.All(nodes, node => Assert.Equal(node.CalcSum(), node.ComposeValue));
                foreach (long n in keys)
                {
                    Assert.Equal((keys.Length + 1L) * keys.Length / 2 - (n * (n + 1) / 2), newRoot.CalcSumGreater(n));
                }
            }
        }


        [Fact]
        public void InsertRotatedOnlySmallRotatedTest()
        {
            var keys = Enumerable.Range(1, 3).ToArray();
            if (keys.Any())
            {
                var root = keys.CreateTreeRotated();
                var nodes = root.InOrderSearch().ToArray();
                var newRoot = nodes.Single(x => x.Parent == null);
                Assert.True(nodes.All(IsLinkCorrect));
                Assert.Equal(keys.OrderBy(x => x).ToArray(), newRoot.InOrderSearch().Select(x => x.Key).ToArray());

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
