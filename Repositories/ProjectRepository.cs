using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ProcessTrackerDbContext _context;

    public ProjectRepository(ProcessTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Projects.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<Project?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Projects.AsQueryable();

        if (includeDetails)
        {
            query = query.Include(p => p.Applications)
                         .Include(p => p.ProcessDefinitionProjectMappings)
                         .ThenInclude(m => m.ProcessDefinition);
        }

        return await query.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Add(project);
        await _context.SaveChangesAsync(cancellationToken);
        return project;
    }

    public async Task UpdateAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Update(project);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Project project, CancellationToken cancellationToken = default)
    {
        _context.Projects.Remove(project);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
