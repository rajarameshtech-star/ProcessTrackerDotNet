using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Repositories;

public class ProcessDefinitionProjectMappingRepository : IProcessDefinitionProjectMappingRepository
{
    private readonly ProcessTrackerDbContext _context;

    public ProcessDefinitionProjectMappingRepository(ProcessTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProcessDefinitionProjectMapping>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessDefinitionProjectMappings
            .AsNoTracking()
            .Include(m => m.ProcessDefinition)
            .Where(m => m.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProcessDefinitionProjectMapping>> GetByProcessDefinitionIdAsync(int processDefinitionId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessDefinitionProjectMappings
            .AsNoTracking()
            .Where(m => m.ProcessDefinitionId == processDefinitionId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ProcessDefinitionProjectMapping?> GetAsync(int processDefinitionId, int projectId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessDefinitionProjectMappings
            .FirstOrDefaultAsync(m => m.ProcessDefinitionId == processDefinitionId && m.ProjectId == projectId, cancellationToken);
    }

    public async Task<bool> ExistsAsync(int processDefinitionId, int projectId, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessDefinitionProjectMappings
            .AnyAsync(m => m.ProcessDefinitionId == processDefinitionId && m.ProjectId == projectId, cancellationToken);
    }

    public async Task<ProcessDefinitionProjectMapping> AddAsync(ProcessDefinitionProjectMapping mapping, CancellationToken cancellationToken = default)
    {
        _context.ProcessDefinitionProjectMappings.Add(mapping);
        await _context.SaveChangesAsync(cancellationToken);
        return mapping;
    }

    public async Task DeleteAsync(ProcessDefinitionProjectMapping mapping, CancellationToken cancellationToken = default)
    {
        _context.ProcessDefinitionProjectMappings.Remove(mapping);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
