
namespace DrawingAnalyzer.Domain.Entities;

public class Drawing
{
    public Guid Id { get; set; }

    public Guid ProjectId { get; set; }

    public string FileName { get; set; } = string.Empty;

    public string StoragePath { get; set; } = string.Empty;

    public string MimeType { get; set; } = "application/pdf";

    public long FileSize { get; set; }

    public DateTime CreatedAt { get; set; }

    public Project? Project { get; set; }

    public ICollection<ProcessingJob> ProcessingJobs { get; set; }
        = new List<ProcessingJob>();
}
