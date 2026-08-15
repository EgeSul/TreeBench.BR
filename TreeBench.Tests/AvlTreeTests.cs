using Xunit;
using TreeBench.BS.Models;

namespace TreeBench.Tests
{
    public class AvlTreeTests
    {
        [Fact]
        public void InsertAndSearch_ShouldReturnTrue_WhenItemExists()
        {
            var tree = new AvlTree();

            tree.Insert(42);

            var result = tree.Search(42);

            Assert.NotNull(result);
        }
    }
}