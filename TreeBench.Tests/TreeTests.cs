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
            Assert.True(tree.Search(42), "AVL Tree: 42 sayısı bulunamadı!");
        }

        [Fact]
        public void RedBlackTree_InsertAndSearch_ShouldReturnTrue()
        {
            var tree = new RedBlackTree();
            tree.Insert(99);
            Assert.True(tree.Search(99), "Red-Black Tree: 99 sayısı bulunamadı!");
        }

        [Fact]
        public void SplayTree_InsertAndSearch_ShouldReturnTrue()
        {
            var tree = new SplayTree();
            tree.Insert(150);
            Assert.True(tree.Search(150), "Splay Tree: 150 sayısı bulunamadı!");
        }
        
        [Fact]
        public void EmptyTree_Search_ShouldReturnFalse()
        {
            var tree = new AvlTree();
            // Ağaca hiçbir şey eklemeden 10 sayısını arıyoruz, false (bulunamadı) dönmeli.
            Assert.False(tree.Search(10), "Boş ağaçta arama yapıldığında hata verdi veya true döndü!");
        }
    }
}