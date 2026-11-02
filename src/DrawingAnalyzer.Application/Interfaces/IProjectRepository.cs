
using DrawingAnalyzer.Domain.Entities;

namespace DrawingAnalyzer.Application.Interfaces;

public interface IProjectRepository
{
    Task AddAsync(
        Project project,
        CancellationToken cancellationToken);

    Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken);

    Task<List<Project>> GetAllAsync(
        CancellationToken cancellationToken);

    Task SaveChangesAsync(
        CancellationToken cancellationToken);
}
