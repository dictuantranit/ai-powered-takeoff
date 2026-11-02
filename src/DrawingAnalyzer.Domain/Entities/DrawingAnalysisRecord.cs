namespace DrawingAnalyzer.Domain.Entities;

public class DrawingAnalysisRecord
{
    public Guid Id { get; set; }

    public Guid DrawingId { get; set; }

    public Guid ProcessingJobId { get; set; }

    public string AnalysisMode { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public int TotalPages { get; set; }

    public int ProcessingTimeMs { get; set; }

    public string ResultJson { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}