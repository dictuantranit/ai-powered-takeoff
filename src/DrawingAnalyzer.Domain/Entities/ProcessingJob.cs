
using DrawingAnalyzer.Domain.Enums;

namespace DrawingAnalyzer.Domain.Entities;

public class ProcessingJob
{
    public Guid Id { get; set; }

    public Guid DrawingId { get; set; }

    public string Status { get; set; }

    public int AttemptCount { get; set; }

    public DateTime? StartedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public Drawing? Drawing { get; set; }

    public ICollection<TakeoffItem> Items { get; set; }
        = new List<TakeoffItem>();
}
