using System;
using System.Collections.Generic;
using System.Diagnostics;
using Serilog;
using TreeBench.BS.Interfaces;
using TreeBench.BS.Models;

namespace TreeBench.BS.Services
{
    // YENİ: TotalSteps (Adım Sayısı) eklendi!
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
        public long TotalSteps { get; set; } // YENİ: Atılan toplam adım sayısı
        public double RamCostKb { get; set; }
    }

    public class BenchmarkService
    {
        public BenchmarkResultModel ExecuteSingleTreeTest(string treeName, IBalancedTree tree, List<int> testData, int mode)
        {
            if (testData == null || testData.Count == 0)
            {
                Log.Warning("⚠️ [{TreeName}] Benchmark skipped: test data empty.", treeName);
                return null;
            }

            tree.ResetMetrics();

            GC.Collect();
            GC.WaitForPendingFinalizers();
            long memoryBefore = GC.GetTotalMemory(true);

            // 1. INSERT TESTİ
            Stopwatch sw = Stopwatch.StartNew();
            foreach (int value in testData)
            {
                tree.Insert(value);
            }
            sw.Stop();
            double insertTimeMs = sw.Elapsed.TotalMilliseconds;

            // 2. SEARCH TESTİ (if kilidi kaldırıldı, her zaman çalışacak)
            double searchTimeMs = 0;
            sw.Restart();
            for (int i = 0; i < 10000; i++)
            {
                tree.Search(testData[i % testData.Count]);
            }
            sw.Stop();
            searchTimeMs = sw.Elapsed.TotalMilliseconds;

            // 3. DELETE TESTİ (if kilidi kaldırıldı, her zaman çalışacak)
            double deleteTimeMs = 0;
            sw.Restart();
            Random rand = new Random();
            for (int i = 0; i < 5000; i++)
            {
                int randomIndex = rand.Next(0, testData.Count);
                tree.Delete(testData[randomIndex]);
            }
            sw.Stop();
            deleteTimeMs = sw.Elapsed.TotalMilliseconds;

            long memoryAfter = GC.GetTotalMemory(true);
            double allocatedKb = Math.Max(0, (memoryAfter - memoryBefore) / 1024.0);

            // Loga Yazdır
            Log.Information("📊 [{TreeName}] Insert: {InsertTime:F2}ms, Search: {SearchTime:F2}ms, Delete: {DeleteTime:F2}ms",
                treeName, insertTimeMs, searchTimeMs, deleteTimeMs);

            // YENİ: DTO'ya dönüş yaparken GetStepCount() metodunu da bağlıyoruz
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
                TotalSteps = tree.GetStepCount(), // YENİ: Temel sınıftan gelecek
                RamCostKb = Math.Round(allocatedKb, 2)
            };
        }
    }
}