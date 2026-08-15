using Serilog;
using TreeBench.BS.Interfaces;
using TreeBench.BS.Models;
using TreeBench.BS.Services;
using TreeBench.API.Hubs;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
    .WriteTo.File("logs/treebench_api_perf.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

try
{
    Log.Information("==================================================");
    Log.Information(" TREEBENCH SIGNALR API ENGINE INITIALIZING...");
    Log.Information("==================================================");

    var builder = WebApplication.CreateBuilder(args);

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll", policy =>
        {
            policy.SetIsOriginAllowed(_ => true)
                  .AllowAnyMethod()
                  .AllowAnyHeader()
                  .AllowCredentials();
        });
    });

    builder.Host.UseSerilog();
    builder.Services.AddControllers();

    builder.Services.AddSignalR();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddOpenApi();

    builder.Services.AddSingleton<DataGenerator>();
    builder.Services.AddSingleton<BenchmarkService>();

    builder.Services.AddTransient<IBalancedTree, AvlTree>();
    builder.Services.AddTransient<IBalancedTree, RedBlackTree>();
    builder.Services.AddTransient<IBalancedTree, SplayTree>();
    builder.Services.AddTransient<IBalancedTree, BPlusTree>();
    builder.Services.AddTransient<IBalancedTree, QuadTree>();

    var app = builder.Build();

    if (app.Environment.IsDevelopment())
    {
        app.MapOpenApi();
    }

    app.UseDefaultFiles();
    app.UseStaticFiles();
    app.UseCors("AllowAll");
    app.MapControllers();

    // YENİ: Canlı yayın frekansımızı (Endpoint) belirledik
    app.MapHub<BenchmarkHub>("/benchmarkHub");

    Log.Information("🚀 TreeBench API + SignalR successfully started!");
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