using DrawingAnalyzer.Application.DTOs;

namespace DrawingAnalyzer.Application.Interfaces;

public interface IAiDrawingAnalyzer
{
    Task<DrawingAnalysisResult> AnalyzeAsync(
        string pdfFilePath,
        Guid drawingId,
        Guid processingJobId,
        CancellationToken cancellationToken);
}