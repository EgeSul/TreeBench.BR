using Xunit;
using TreeBench.BS.Services;
using TreeBench.BS.Models;
using System.Collections.Generic;

namespace TreeBench.Tests
{
    public class BenchmarkServiceTests
    {
        [Fact]
        public void ExecuteSingleTreeTest_ShouldRunSuccessfully_WithDirectData()
        {
            var service = new BenchmarkService(); 
            var tree = new AvlTree(); 
            var dummyData = new List<int> { 10, 20, 30 };
            int mode = 1;

            var result = service.ExecuteSingleTreeTest("AVL Tree", tree, dummyData, mode);

            Assert.NotNull(result);
            Assert.True(result.InsertTimeMs >= 0, "Ekleme süresi hesaplanamadı!");
            Assert.Equal("AVL Tree", result.TreeName);
        }
    }
}