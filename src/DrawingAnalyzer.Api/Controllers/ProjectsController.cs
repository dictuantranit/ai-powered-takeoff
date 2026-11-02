
using DrawingAnalyzer.Application.DTOs;
using DrawingAnalyzer.Application.Interfaces;
using DrawingAnalyzer.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DrawingAnalyzer.Api.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectsController(
    IProjectRepository projectRepository) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status201Created)]
    public async Task<ActionResult<ProjectResponse>> Create(
        CreateProjectRequest request,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Description = request.Description?.Trim(),
            CreatedAt = now,
            UpdatedAt = now
        };

        await projectRepository.AddAsync(project, cancellationToken);
        await projectRepository.SaveChangesAsync(cancellationToken);

        var response = ToResponse(project);

        return CreatedAtAction(
            nameof(GetById),
            new { id = project.Id },
            response);
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ProjectResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ProjectResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        var projects = await projectRepository.GetAllAsync(cancellationToken);

        return Ok(projects.Select(ToResponse).ToList());
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProjectResponse>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var project = await projectRepository.GetByIdAsync(id, cancellationToken);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(ToResponse(project));
    }

    private static ProjectResponse ToResponse(Project project)
    {
        return new ProjectResponse(
            project.Id,
            project.Name,
            project.Description,
            project.CreatedAt,
            project.UpdatedAt);
    }
}
