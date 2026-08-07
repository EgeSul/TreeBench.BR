using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TreeBench.BS.Interfaces;
using TreeBench.BS.Services;

namespace TreeBench.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BenchmarkController : ControllerBase
    {
        private readonly IEnumerable<IBalancedTree> _trees;
        private readonly DataGenerator _dataGenerator;
        private readonly BenchmarkService _benchmarkService;

        public BenchmarkController(
            IEnumerable<IBalancedTree> trees,
            DataGenerator dataGenerator,
            BenchmarkService benchmarkService)
        {
            _trees = trees;
            _dataGenerator = dataGenerator;
            _benchmarkService = benchmarkService;
        }

        [HttpPost("run")]
        public async Task<IActionResult> RunBenchmark([FromQuery] int mode = 1)
        {
            var testData = await Task.Run(() => _dataGenerator.FetchDataFromSql());

            if (testData == null || testData.Count == 0)
            {
                testData = new List<int>();
                Random rand = new Random();
                for (int i = 0; i < 100000; i++) testData.Add(rand.Next(1, 1000000));
            }

            var results = await Task.Run(() =>
            {
                var list = new List<BenchmarkResultModel>();
                foreach (var tree in _trees)
                {
                    var res = _benchmarkService.ExecuteSingleTreeTest(tree.GetType().Name, tree, testData, mode);
                    if (res != null) list.Add(res);
                }
                return list;
            });

            // 200 OK 
            return Ok(results);
        }
    }
}