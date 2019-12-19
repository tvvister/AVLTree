using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AutoFixture;
using Xunit;

namespace AVLTree.UnitTests
{
    /// <summary>
    /// Integration tests for control count, height and sorting of some operation with Tree
    /// </summary>
    public class RotationOperationsTests
    {
        private readonly Fixture _fixture = new Fixture();
        private int[] GetKeys(int count)
        {
            return _fixture.CreateMany<int>(count).ToArray();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(100)]
        [InlineData(1000)]
        //[InlineData(3000)] slow
        [InlineData(1521)]
        [InlineData(931)]
        [InlineData(991)]
        [InlineData(817)]
        public void InsertAndRemoveRotatedOperationsTest(int count)
        {
            var keys = GetKeys(count); //Arrange;

            var set = new SortedSet<int>();
            TreeNode root = null;
            foreach (var key in keys)
            {
                Assert.Equal(set, root.InOrderSearch().Select(x => x.Key));
                if (set.Contains(key))
                {
                    //set.Remove(key);
                }
                else
                {
                    set.Add(key);
                }
                if (root?.TryFind(key, out TreeNode foundNode) == true)
                {
                    //root = root.RemoveRotated(key);
                }
                else
                {
                    root = root.Add(key);
                }
                Assert.Equal(set, root.InOrderSearch().Select(x => x.Key));
                var maxHeight = root.InOrderSearch().Select(x => x.Height).Append(0).Max();
                Assert.InRange(maxHeight, 0, Math.Floor(1.45 * Math.Log(keys.Length + 2, 2)));
            }

            foreach (var key in keys.OrderBy(x => (31*x + 41) % 91))
            {
                Assert.Equal(set, root.InOrderSearch().Select(x => x.Key));
                if (set.Contains(key))
                {
                    set.Remove(key);
                }
                
                if (root?.TryFind(key, out TreeNode foundNode) == true)
                {
                    root = root.RemoveRotated(key);
                }
                
                Assert.Equal(set, root.InOrderSearch().Select(x => x.Key));
                var maxHeight = root.InOrderSearch().Select(x => x.Height).Append(0).Max();
                Assert.InRange(maxHeight, 0, Math.Floor(1.45 * Math.Log(keys.Length + 2, 2)));
            }

        }
    }
}
