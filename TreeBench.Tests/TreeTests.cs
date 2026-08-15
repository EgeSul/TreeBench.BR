using Xunit;
using TreeBench.BS.Models;

namespace TreeBench.Tests
{
    public class TreeTests
    {
        [Fact]
        public void AvlTree_InsertAndSearch_ShouldReturnTrue()
        {
            var tree = new AvlTree();
            tree.Insert(42);
            Assert.True(tree.Search(42), "AVL Tree: Number 42 not found!");
        }

        [Fact]
        public void RedBlackTree_InsertAndSearch_ShouldReturnTrue()
        {
            var tree = new RedBlackTree();
            tree.Insert(99);
            Assert.True(tree.Search(99), "Red-Black Tree: Number 99 not found!");
        }

        [Fact]
        public void SplayTree_InsertAndSearch_ShouldReturnTrue()
        {
            var tree = new SplayTree();
            tree.Insert(150);
            Assert.True(tree.Search(150), "Splay Tree: Number 150 not found!");
        }
        
        [Fact]
        public void EmptyTree_Search_ShouldReturnFalse()
        {
            var tree = new AvlTree();
            Assert.False(tree.Search(10), "Empty tree: Search returned true instead of false!");
        }
    }
}