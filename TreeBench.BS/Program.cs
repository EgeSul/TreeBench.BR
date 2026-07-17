using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using TreeBench.BS.Interfaces;
using TreeBench.BS.Models;
using TreeBench.BS.Services;

namespace TreeBench.BS
{
    class Program
    {
        static void Main(string[] args)
        {
            /// <summary>
            /// [X] v1.5.0 - .NET Dependency Injection 
            /// FEDO: Implemented Dependency Injection using Microsoft.Extensions.DependencyInjection for better modularity and testability.
            /// </summary>

            var services = new ServiceCollection();
            services.AddSingleton<DataGenerator>();
            services.AddSingleton<BenchmarkService>();
            services.AddTransient<IBalancedTree, AvlTree>();
            services.AddTransient<IBalancedTree, RedBlackTree>();
            services.AddTransient<IBalancedTree, SplayTree>();


            var serviceProvider = services.BuildServiceProvider();


            /// [X] v1.0.0 - Set Console Title
            Console.Title = "TreeBench v1.5 - Advanced Data Structures Lab";

            Console.WriteLine("==================================================");
            Console.WriteLine("🚀 TREEBENCH PERFORMANCE LAB INITIALIZING...");
            Console.WriteLine("==================================================");

            DataGenerator dataGenerator = serviceProvider.GetRequiredService<DataGenerator>();
            BenchmarkService benchmarkService = serviceProvider.GetRequiredService<BenchmarkService>();

            Console.WriteLine("\n⏳ Fetching 100,000 records from SQL Server (SQLEXPRESS)...");
            List<int> sqlData = dataGenerator.FetchDataFromSql();

            if (sqlData == null || sqlData.Count == 0)
            {
                Console.WriteLine("[!] Error: No data retrieved from SQL Server. Check ConnectionString.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine($"[+] Successfully loaded {sqlData.Count:N0} records into C# Memory!");

            var managedTrees = serviceProvider.GetServices<IBalancedTree>();

            Console.WriteLine("\n==================================================");
            Console.WriteLine("⚔️  BENCHMARK COMPETITION STARTING...");
            Console.WriteLine("==================================================\n");

            foreach (var tree in managedTrees)
            {
                benchmarkService.ExecuteTreeTest(tree.GetType().Name, tree, sqlData);
            }

            Console.WriteLine("==================================================");
            Console.WriteLine("🏁 BENCHMARK COMPLETED. PRESS ENTER TO EXIT.");
            Console.WriteLine("==================================================");
            Console.ReadLine();
        }
    }
}