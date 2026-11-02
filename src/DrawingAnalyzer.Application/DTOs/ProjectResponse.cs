
namespace DrawingAnalyzer.Application.DTOs;

public record ProjectResponse(
    Guid Id,
    string Name,
    string? Description,
    DateTime CreatedAt,
    DateTime UpdatedAt);
