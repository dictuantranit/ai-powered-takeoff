using DrawingAnalyzer.Application.DTOs;
using DrawingAnalyzer.Application.Interfaces;
using DrawingAnalyzer.Domain.Entities;
using DrawingAnalyzer.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DrawingAnalyzer.Api.Controllers;

[ApiController]
[Route("api/projects/{projectId:guid}/drawings")]
public class DrawingsController(
    DrawingAnalyzerDbContext dbContext,
    IFileStorage fileStorage) : ControllerBase
{
    private const long MaxFileSize = 20 * 1024 * 1024;

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(
        typeof(UploadDrawingResponse),
        StatusCodes.Status202Accepted)]
    public async Task<ActionResult<UploadDrawingResponse>> Upload(
        Guid projectId,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
        {
            return BadRequest(new
            {
                message = "Please select a non-empty PDF file."
            });
        }

        if (file.Length > MaxFileSize)
        {
            return BadRequest(new
            {
                message = "The maximum file size is 20 MB."
            });
        }

        // Check the extension, but do not rely on it alone
        // as proof that the file is a valid PDF.
        if (!string.Equals(
                Path.GetExtension(file.FileName),
                ".pdf",
                StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest(new
            {
                message = "Only PDF files are supported."
            });
        }

        var projectExists = await dbContext.Projects
            .AnyAsync(
                p => p.Id == projectId,
                cancellationToken);

        if (!projectExists)
        {
            return NotFound(new
            {
                message = "Project not found."
            });
        }

        // Check the PDF signature (%PDF-).
        await using var input = file.OpenReadStream();

        var header = new byte[5];
        var bytesRead = await input.ReadAsync(
            header.AsMemory(0, header.Length),
            cancellationToken);

        if (bytesRead != 5 ||
            !header.AsSpan().SequenceEqual(
                "%PDF-"u8))
        {
            return BadRequest(new
            {
                message = "The uploaded file is not a valid PDF."
            });
        }

        // Reset the stream after checking its header.
        input.Position = 0;

        var now = DateTime.UtcNow;
        var drawingId = Guid.NewGuid();
        var jobId = Guid.NewGuid();

        // Never use the user-provided filename as the storage path.
        var storageFileName = $"{drawingId:N}.pdf";

        string storagePath;

        try
        {
            storagePath = await fileStorage.SaveAsync(
                input,
                storageFileName,
                cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (IOException)
        {
            return StatusCode(
                StatusCodes.Status500InternalServerError,
                new { message = "Unable to save the uploaded file." });
        }

        var drawing = new Drawing
        {
            Id = drawingId,
            ProjectId = projectId,
            FileName = Path.GetFileName(file.FileName),
            StoragePath = storagePath,
            MimeType = "application/pdf",
            FileSize = file.Length,
            CreatedAt = now
        };

        var job = new ProcessingJob
        {
            Id = jobId,
            DrawingId = drawingId,
            Status = "Queued",
            AttemptCount = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        try
        {
            await using var transaction =
                await dbContext.Database.BeginTransactionAsync(
                    cancellationToken);

            dbContext.Drawings.Add(drawing);
            dbContext.ProcessingJobs.Add(job);

            await dbContext.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            // Avoid leaving an orphaned file when DB persistence fails.
            await fileStorage.DeleteAsync(
                storagePath,
                CancellationToken.None);

            throw;
        }

        var response = new UploadDrawingResponse(
            drawingId,
            jobId,
            drawing.FileName,
            job.Status,
            "Drawing uploaded. Processing job queued.");

        return Accepted(response);
    }
}