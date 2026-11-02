using DrawingAnalyzer.Worker;
using DrawingAnalyzer.Worker.Workers;
using DrawingAnalyzer.Application.Interfaces;
using DrawingAnalyzer.Infrastructure.AI;
using DrawingAnalyzer.Infrastructure.Persistence;
using DrawingAnalyzer.Infrastructure.Storage;
using Microsoft.EntityFrameworkCore;

var builder = Host.CreateApplicationBuilder(args);

// 1. Get MySQL connection string
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is missing.");

// 2. Register DbContext
builder.Services.AddDbContext<DrawingAnalyzerDbContext>(options =>
{
    options.UseMySQL(connectionString);
});

//builder.Services.AddHostedService<Worker>();

builder.Services.AddHttpClient<
    IAiDrawingAnalyzer,
    PythonAiDrawingAnalyzer>(
    client =>
    {
        client.BaseAddress = new Uri(
            builder.Configuration["PythonAI:BaseUrl"]
            ?? "http://127.0.0.1:8000/");

        client.Timeout = TimeSpan.FromSeconds(180);
    });


builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

builder.Services.AddHostedService<DrawingProcessingWorker>();

var host = builder.Build();
host.Run();
