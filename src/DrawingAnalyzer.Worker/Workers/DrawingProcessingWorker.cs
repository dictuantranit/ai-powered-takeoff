using DrawingAnalyzer.Application.Interfaces;
using DrawingAnalyzer.Domain.Entities;
using DrawingAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DrawingAnalyzer.Worker.Workers;

public sealed class DrawingProcessingWorker(
    IServiceScopeFactory scopeFactory,
    IConfiguration configuration,
    ILogger<DrawingProcessingWorker> logger)
    : BackgroundService
{
    private readonly int _pollingInterval =
        configuration.GetValue<int>(
            "Worker:PollingIntervalSeconds", 5);

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        logger.LogInformation("Drawing processing worker started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var processed = await ProcessNextJobAsync(
                    stoppingToken);

                if (!processed)
                {
                    await Task.Delay(
                        TimeSpan.FromSeconds(_pollingInterval),
                        stoppingToken);
                }
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(
                    ex,
                    "Unexpected error in drawing processing worker.");

                await Task.Delay(
                    TimeSpan.FromSeconds(_pollingInterval),
                    stoppingToken);
            }
        }

        logger.LogInformation("Drawing processing worker stopped.");
    }

    private async Task<bool> ProcessNextJobAsync(
        CancellationToken cancellationToken)
    {
        using var scope = scopeFactory.CreateScope();

        var dbContext = scope.ServiceProvider
            .GetRequiredService<DrawingAnalyzerDbContext>();

        var analyzer = scope.ServiceProvider
            .GetRequiredService<IAiDrawingAnalyzer>();

        var job = await dbContext.ProcessingJobs
            .Include(x => x.Drawing)
            .Where(x => x.Status == "Queued")
            .OrderBy(x => x.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (job is null)
        {
            return false;
        }

        if (job.Drawing is null ||
            !File.Exists(job.Drawing.StoragePath))
        {
            job.Status = "Failed";
            job.ErrorMessage = "Drawing file was not found.";
            job.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        job.Status = "Processing";
        job.StartedAt = DateTime.UtcNow;
        job.AttemptCount++;
        job.ErrorMessage = null;

        await dbContext.SaveChangesAsync(cancellationToken);

        try
        {
            logger.LogInformation(
                "Processing job {JobId}, drawing {DrawingId}",
                job.Id,
                job.DrawingId);

            var result = await analyzer.AnalyzeAsync(
                job.Drawing.StoragePath,
                job.DrawingId,
                job.Id,
                cancellationToken);

            ValidateResult(result);

            var analysisRecord = new DrawingAnalysisRecord
            {
                Id = Guid.NewGuid(),
                DrawingId = job.DrawingId,
                ProcessingJobId = job.Id,
                AnalysisMode = result.AnalysisMode,
                Status = result.Status,
                TotalPages = result.TotalPages,
                ProcessingTimeMs = result.ProcessingTimeMs,
                ResultJson = JsonSerializer.Serialize(
                    result,
                    new JsonSerializerOptions(JsonSerializerDefaults.Web)),
                CreatedAt = DateTime.UtcNow
            };

            dbContext.DrawingAnalysisRecords.Add(analysisRecord);

            foreach (var item in result.Items)
            {
                dbContext.TakeoffItems.Add(new TakeoffItem
                {
                    Id = Guid.NewGuid(),
                    ProcessingJobId = job.Id,
                    ObjectType = item.ObjectType,
                    ObjectName = item.ObjectName,
                    Quantity = item.Quantity,
                    Confidence = item.Confidence,
                    CreatedAt = DateTime.UtcNow
                });
            }

            job.Status = "Completed";
            job.CompletedAt = DateTime.UtcNow;
            job.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Job {JobId} completed with {ItemCount} takeoff items.",
                job.Id,
                result.Items.Count);
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to process job {JobId}",
                job.Id);

            job.Status = "Failed";
            job.ErrorMessage = ex.Message.Length > 2000
                ? ex.Message[..2000]
                : ex.Message;

            job.UpdatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(
                CancellationToken.None);
        }

        return true;
    }

    private static void ValidateResult(
        Application.DTOs.DrawingAnalysisResult result)
    {
        if (result.Items is null)
        {
            throw new InvalidOperationException(
                "AI returned an empty items collection.");
        }

        var allowedTypes = new[]
        {
            "Door",
            "Window",
            "Room"
        };

        foreach (var item in result.Items)
        {
            if (!allowedTypes.Contains(
                    item.ObjectType,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException(
                    $"Unsupported object type: {item.ObjectType}");
            }

            if (string.IsNullOrWhiteSpace(item.ObjectName))
            {
                throw new InvalidOperationException(
                    "Object name cannot be empty.");
            }

            if (item.Quantity < 0)
            {
                throw new InvalidOperationException(
                    "Quantity cannot be negative.");
            }

            if (item.Confidence is < 0 or > 1)
            {
                throw new InvalidOperationException(
                    "Confidence must be between 0 and 1.");
            }
        }
    }
}