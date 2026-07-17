using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using Serilog;
using TreeBench.BS.Interfaces;
using TreeBench.BS.Models;
using TreeBench.BS.Services;

namespace TreeBench.BS
{
    class Program
    {
        static void Main(string[] args)
        {
            // --- SERILOG BOOTSTRAP CONFIGURATION ---
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File("logs/treebench_perf.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            Console.Title = "TreeBench v2.0 - Enterprise Performance Lab";

            Log.Information("==================================================");
            Log.Information(" TREEBENCH PERFORMANCE LAB INITIALIZING (v2.0)...");
            Log.Information("==================================================");

            var services = new ServiceCollection();
            services.AddSingleton<DataGenerator>();
            services.AddSingleton<BenchmarkService>();
            services.AddTransient<IBalancedTree, AvlTree>();
            services.AddTransient<IBalancedTree, RedBlackTree>();
            services.AddTransient<IBalancedTree, SplayTree>();
            services.AddTransient<IBalancedTree, BPlusTree>();
            services.AddTransient<IBalancedTree, QuadTree>();

            var serviceProvider = services.BuildServiceProvider();

            DataGenerator dataGenerator = serviceProvider.GetRequiredService<DataGenerator>();
            BenchmarkService benchmarkService = serviceProvider.GetRequiredService<BenchmarkService>();

            Log.Information("⏳ Fetching 100,000 records from SQL Server (SQLEXPRESS)...");
            List<int> sqlData = dataGenerator.FetchDataFromSql();

            // --- V2.0.0 FAULT TOLERANCE & FALLBACK MECHANISM ---
            if (sqlData != null && sqlData.Count > 0)
            {
                Log.Information("[+] Successfully loaded {RecordCount:N0} records from SQL Server into C# Memory!", sqlData.Count);
            }
            else
            {
                Log.Warning("[!] WARNING: SQL Server source not found or connection failed!");
                Log.Warning("[!] SWITCHING TO ENTERPRISE FALLBACK MODE: Generating in-memory fallback dataset...");

                sqlData = new List<int>();
                Random rand = new Random();

                for (int i = 0; i < 100000; i++)
                {
                    sqlData.Add(rand.Next(1, 1000000));
                }

                Log.Information("[+] Fallback dataset successfully initialized with {RecordCount:N0} mock records!", sqlData.Count);
            }

            // ======================================
            //             INTERFACES
            // ======================================
            Log.Information("\n==================================================");
            Log.Information("   SELECT BENCHMARK MODE:");
            Log.Information("1. Insert & Search Performance Test");
            Log.Information("2. Heavy Ingestion & Deletion Stress Test");
            Log.Information("3. Topological Analysis & Metrics");
            Log.Information("==================================================");

            Console.Write("👉 Enter mode number (1-3): ");
            string selection = Console.ReadLine();

            int benchmarkMode = 1; // Default: Standard Mode
            if (int.TryParse(selection, out int result) && result >= 1 && result <= 3)
            {
                benchmarkMode = result;
            }

            Log.Information("\n==================================================");
            Log.Information("⚔️  BENCHMARK COMPETITION STARTING...");
            Log.Information("==================================================\n");

            var managedTrees = serviceProvider.GetServices<IBalancedTree>();

            foreach (var tree in managedTrees)
            {
                benchmarkService.ExecuteTreeTest(tree.GetType().Name, tree, sqlData, benchmarkMode);
            }

            Log.Information("==================================================");
            Log.Information("🏁 BENCHMARK COMPLETED. PRESS ENTER TO EXIT.");
            Log.Information("==================================================");
            Console.ReadLine();

            Log.CloseAndFlush();
        }
    }
}