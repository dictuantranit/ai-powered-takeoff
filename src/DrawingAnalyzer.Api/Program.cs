using DrawingAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using DrawingAnalyzer.Application.Interfaces;
using DrawingAnalyzer.Infrastructure.Persistence.Repositories;
using DrawingAnalyzer.Infrastructure.Storage;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration
    .GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is missing.");

builder.Services.AddDbContext<DrawingAnalyzerDbContext>(options =>
{
    options.UseMySQL(connectionString!);
});
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddSingleton<IFileStorage, LocalFileStorage>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();