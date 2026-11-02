
using DrawingAnalyzer.Application.Interfaces;
using DrawingAnalyzer.Domain.Entities;
using DrawingAnalyzer.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DrawingAnalyzer.Infrastructure.Persistence.Repositories;

public class ProjectRepository(
    DrawingAnalyzerDbContext dbContext) : IProjectRepository
{
    public async Task AddAsync(
        Project project,
        CancellationToken cancellationToken)
    {
        await dbContext.Projects.AddAsync(project, cancellationToken);
    }

    public Task<Project?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken)
    {
        return dbContext.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public Task<List<Project>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return dbContext.Projects
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task SaveChangesAsync(
        CancellationToken cancellationToken)
    {
        return dbContext.SaveChangesAsync(cancellationToken);
    }
}
