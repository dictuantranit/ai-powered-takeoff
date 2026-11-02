
namespace DrawingAnalyzer.Domain.Entities;

public class TakeoffItem
{
    public Guid Id { get; set; }

    public Guid ProcessingJobId { get; set; }

    public string ObjectType { get; set; } = string.Empty;

    public string? ObjectName { get; set; }

    public decimal Quantity { get; set; }

    public decimal? Confidence { get; set; }

    public string? MetadataJson { get; set; }

    public DateTime CreatedAt { get; set; }

    public ProcessingJob? ProcessingJob { get; set; }
}
