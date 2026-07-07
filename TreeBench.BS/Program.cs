using System;
using System.Collections.Generic;
using TreeBench.BS.Models;
using TreeBench.BS.Services;

namespace TreeBench.BS
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "TreeBench v1.0 - Advanced Data Structures Lab";

            Console.WriteLine("==================================================");
            Console.WriteLine("🚀 TREEBENCH PERFORMANCE LAB INITIALIZING...");
            Console.WriteLine("==================================================");

            DataGenerator dataGenerator = new DataGenerator();
            BenchmarkService benchmarkService = new BenchmarkService();

            Console.WriteLine("\n⏳ Fetching 100,000 records from SQL Server (SQLEXPRESS)...");
            List<int> sqlData = dataGenerator.FetchDataFromSql();

            if (sqlData == null || sqlData.Count == 0)
            {
                Console.WriteLine("[!] Error: No data retrieved from SQL Server. Check ConnectionString.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine($"[+] Successfully loaded {sqlData.Count:N0} records into C# Memory!");

            var avlTree = new AvlTree();
            var rbTree = new RedBlackTree();
            var splayTree = new SplayTree();

            Console.WriteLine("\n==================================================");
            Console.WriteLine("⚔️  BENCHMARK COMPETITION STARTING...");
            Console.WriteLine("==================================================\n");

            benchmarkService.ExecuteTreeTest("AVL Tree", avlTree, sqlData);
            benchmarkService.ExecuteTreeTest("Red-Black Tree", rbTree, sqlData);
            benchmarkService.ExecuteTreeTest("Splay Tree", splayTree, sqlData);

            Console.WriteLine("==================================================");
            Console.WriteLine("🏁 BENCHMARK COMPLETED. PRESS ENTER TO EXIT.");
            Console.WriteLine("==================================================");
            Console.ReadLine();
        }
    }
}