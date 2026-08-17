using System;
using System.Collections.Generic;
using System.Diagnostics;
using Serilog;
using TreeBench.BS.Interfaces;
using TreeBench.BS.Models;

namespace TreeBench.BS.Services
{
    public class BenchmarkResultModel
    {
        public string TreeName { get; set; } = string.Empty;
        public int Mode { get; set; }
        public int NodeCount { get; set; }
        public double InsertTimeMs { get; set; }
        public double SearchTimeMs { get; set; }
        public double DeleteTimeMs { get; set; }
        public int MaxDepth { get; set; }
        public int MinDepth { get; set; }
        public int TotalRotations { get; set; }
        public long TotalSteps { get; set; }
        public double RamCostKb { get; set; }
    }

    public class BenchmarkService
    {
        public BenchmarkResultModel ExecuteSingleTreeTest(string treeName, IBalancedTree tree, List<int> testData, int mode, Action<string, string, int> onProgress = null)
        {
            if (testData == null || testData.Count == 0) return null;

            tree.ResetMetrics();
            GC.Collect();
            GC.WaitForPendingFinalizers();
            long memoryBefore = GC.GetTotalMemory(true);

            int total = testData.Count;
            int notifyStep = Math.Max(1, total / 10);

            Stopwatch sw = Stopwatch.StartNew();
            for (int i = 0; i < total; i++)
            {
                tree.Insert(testData[i]);
                if (i % notifyStep == 0)
                    onProgress?.Invoke(treeName, "Insert", (i * 100) / total);
            }
            sw.Stop();
            double insertTimeMs = sw.Elapsed.TotalMilliseconds;

            double searchTimeMs = 0;
            sw.Restart();
            int searchCount = 10000;
            for (int i = 0; i < searchCount; i++)
            {
                tree.Search(testData[i % total]);
                if (i % (searchCount / 10) == 0)
                    onProgress?.Invoke(treeName, "Search", (i * 100) / searchCount);
            }
            sw.Stop();
            searchTimeMs = sw.Elapsed.TotalMilliseconds;

            double deleteTimeMs = 0;
            sw.Restart();
            int deleteCount = 5000;
            Random rand = new Random();
            for (int i = 0; i < deleteCount; i++)
            {
                int randomIndex = rand.Next(0, testData.Count);
                tree.Delete(testData[randomIndex]);
                if (i % (deleteCount / 10) == 0)
                    onProgress?.Invoke(treeName, "Delete", (i * 100) / deleteCount);
            }
            sw.Stop();
            deleteTimeMs = sw.Elapsed.TotalMilliseconds;

            long memoryAfter = GC.GetTotalMemory(true);
            double allocatedKb = Math.Max(0, (memoryAfter - memoryBefore) / 1024.0);

            onProgress?.Invoke(treeName, "Complete", 100);

            return new BenchmarkResultModel
            {
                TreeName = treeName,
                Mode = mode,
                NodeCount = tree.Count,
                InsertTimeMs = Math.Round(insertTimeMs, 4),
                SearchTimeMs = Math.Round(searchTimeMs, 4),
                DeleteTimeMs = Math.Round(deleteTimeMs, 4),
                MaxDepth = tree.GetMaxDepth(),
                MinDepth = tree.GetMinDepth(),
                TotalRotations = tree.GetRotationsCount(),
                TotalSteps = tree.GetStepCount(),
                RamCostKb = Math.Round(allocatedKb, 2)
            };
        }
    }
}