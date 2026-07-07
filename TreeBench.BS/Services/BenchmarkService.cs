using System;
using System.Collections.Generic;
using System.Diagnostics;
using TreeBench.BS.Interfaces;
using TreeBench.BS.Models;

namespace TreeBench.BS.Services
{
    public class BenchmarkService
    {
        /// <summary>
        /// The tree passed as a parameter is tested against the data from SQL and returns metrics.
        /// </summary>
        public void ExecuteTreeTest(string treeName, IBalancedTree tree, List<int> testData)
        {
            tree.ResetMetrics();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            long memoryBefore = GC.GetTotalMemory(true);

            Stopwatch sw = new Stopwatch();

            sw.Start();
            foreach (int value in testData)
            {
                tree.Insert(value);
            }
            sw.Stop();
            double insertTimeMs = sw.Elapsed.TotalMilliseconds;

            sw.Restart();
            for (int i = 0; i < 10000; i++)
            {
                tree.Search(testData[i % testData.Count]);
            }
            sw.Stop();
            double searchTimeMs = sw.Elapsed.TotalMilliseconds;

            long memoryAfter = GC.GetTotalMemory(true);
            double allocatedKb = (memoryAfter - memoryBefore) / 1024.0;
            if (allocatedKb < 0) allocatedKb = 0;

            Console.WriteLine($"------------------------------------------------");
            Console.WriteLine($"📊 [{treeName.ToUpper()}] PERFORMANS RAPORU");
            Console.WriteLine($"------------------------------------------------");
            Console.WriteLine($"🔹 Total Node Count          : {tree.Count:N0}");
            Console.WriteLine($"🔹 Addition Time    (Insert) : {insertTimeMs:F4} ms");
            Console.WriteLine($"🔹 Search Duration  (Search) : {searchTimeMs:F4} ms");
            Console.WriteLine($"🔹 Maximum Depth    (Height) : {tree.GetMaxDepth()} kat");
            Console.WriteLine($"🔹 Minimum Depth    (Height) : {tree.GetMinDepth()} kat");
            Console.WriteLine($"🔹 Total Rotations           : {tree.GetRotationsCount():N0}");
            Console.WriteLine($"🔹 RAM Cost (Approximate): {allocatedKb:F2} KB");
            Console.WriteLine($"------------------------------------------------\n");
        }
    }
}