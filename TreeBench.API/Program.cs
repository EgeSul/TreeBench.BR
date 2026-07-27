using Serilog;
using TreeBench.BS.Interfaces;
using TreeBench.BS.Models;
using TreeBench.BS.Services;

// --- SERILOG CONFIGURATION FOR API ---
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/treebench_api_perf.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("==================================================");
    Log.Information(" TREEBENCH API ENGINE INITIALIZING...");
    Log.Information("==================================================");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog();

    // --- CONTROLLERS & OPENAPI (SWAGGER) ---
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    // --- TREEBENCH.BS CORE SERVICES (DEPENDENCY INJECTION) ---
    builder.Services.AddSingleton<DataGenerator>();
    builder.Services.AddSingleton<BenchmarkService>();

    // Managed Trees
    builder.Services.AddTransient<IBalancedTree, AvlTree>();
    builder.Services.AddTransient<IBalancedTree, RedBlackTree>();
    builder.Services.AddTransient<IBalancedTree, SplayTree>();
    builder.Services.AddTransient<IBalancedTree, BPlusTree>();
    builder.Services.AddTransient<IBalancedTree, QuadTree>();

    var app = builder.Build();

    // --- HTTP REQUEST PIPELINE ---
    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseHttpsRedirection();
    app.UseAuthorization();
    app.MapControllers();

    Log.Information("🚀 TreeBench API successfully started and listening for requests.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "❌ TreeBench API terminated unexpectedly!");
}
finally
{
    Log.CloseAndFlush();
}