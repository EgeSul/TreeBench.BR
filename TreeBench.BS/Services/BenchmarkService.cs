using System;
using System.Collections.Generic;
using System.Diagnostics;
using Serilog;
using TreeBench.BS.Interfaces;

namespace TreeBench.BS.Services
{
    public class BenchmarkService
    {
        public void ExecuteTreeTest(string treeName, IBalancedTree tree, List<int> testData, int mode)
        {
            tree.ResetMetrics();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            long memoryBefore = GC.GetTotalMemory(true);

            Stopwatch sw = new Stopwatch();
            double insertTimeMs = 0;
            double searchTimeMs = 0;
            double deleteTimeMs = 0;

            sw.Start();
            foreach (int value in testData)
            {
                tree.Insert(value);
            }
            sw.Stop();
            insertTimeMs = sw.Elapsed.TotalMilliseconds;

            if (mode == 1)
            {
                sw.Restart();
                for (int i = 0; i < 10000; i++)
                {
                    tree.Search(testData[i % testData.Count]);
                }
                sw.Stop();
                searchTimeMs = sw.Elapsed.TotalMilliseconds;
            }
            else if (mode == 2) 
            {
                sw.Restart();
                Random rand = new Random();
                for (int i = 0; i < 5000; i++)
                {
                    int randomIndex = rand.Next(0, testData.Count);
                    tree.Delete(testData[randomIndex]); 
                }
                sw.Stop();
                deleteTimeMs = sw.Elapsed.TotalMilliseconds;
            }

            long memoryAfter = GC.GetTotalMemory(true);
            double allocatedKb = (memoryAfter - memoryBefore) / 1024.0;
            if (allocatedKb < 0) allocatedKb = 0;

            Log.Information("------------------------------------------------");
            Log.Information("📊 [{TreeName}] PERFORMANCE REPORT (Mode: {Mode})", treeName.ToUpper(), mode);
            Log.Information("------------------------------------------------");
            Log.Information("🔹 Active Node Count         : {Count:N0}", tree.Count);
            Log.Information("🔹 Addition Time   (Insert)  : {InsertTime:F4} ms", insertTimeMs);

            if (mode == 1)
                Log.Information("🔹 Search Duration (Search)  : {SearchTime:F4} ms", searchTimeMs);
            if (mode == 2)
                Log.Information("🔹 Deletion Time   (Delete)  : {DeleteTime:F4} ms", deleteTimeMs);

            Log.Information("🔹 Maximum Depth   (Height)  : {MaxDepth} kat", tree.GetMaxDepth());
            Log.Information("🔹 Minimum Depth   (Height)  : {MinDepth} kat", tree.GetMinDepth());
            Log.Information("🔹 Total Rotations           : {Rotations:N0}", tree.GetRotationsCount());
            Log.Information("🔹 RAM Cost (Approximate)    : {RamCost:F2} KB", allocatedKb);
            Log.Information("------------------------------------------------\n");
        }
    }
}