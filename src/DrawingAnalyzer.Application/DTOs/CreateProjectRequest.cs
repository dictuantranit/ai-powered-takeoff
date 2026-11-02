
using System.ComponentModel.DataAnnotations;

namespace DrawingAnalyzer.Application.DTOs;

public class CreateProjectRequest
{
    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Name { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }
}
