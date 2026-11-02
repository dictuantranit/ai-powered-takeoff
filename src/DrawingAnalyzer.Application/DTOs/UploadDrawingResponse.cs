
namespace DrawingAnalyzer.Application.DTOs;

public record UploadDrawingResponse(
    Guid DrawingId,
    Guid JobId,
    string FileName,
    string Status,
    string Message);
