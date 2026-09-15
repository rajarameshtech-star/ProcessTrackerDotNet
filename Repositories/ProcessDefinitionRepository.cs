using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Repositories;

public class ProcessDefinitionRepository : IProcessDefinitionRepository
{
    private readonly ProcessTrackerDbContext _context;

    public ProcessDefinitionRepository(ProcessTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProcessDefinition>> GetAllAsync(bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _context.ProcessDefinitions.AsNoTracking();
        if (activeOnly)
        {
            query = query.Where(pd => pd.IsActive);
        }
        return await query.ToListAsync(cancellationToken);
    }

    public async Task<ProcessDefinition?> GetByIdAsync(int id, bool includeFields = false, CancellationToken cancellationToken = default)
    {
        var query = _context.ProcessDefinitions.AsQueryable();
        if (includeFields)
        {
            query = query.Include(pd => pd.ProcessFields.OrderBy(f => f.SortOrder));
        }
        return await query.FirstOrDefaultAsync(pd => pd.Id == id, cancellationToken);
    }

    public async Task<ProcessDefinition?> GetByCodeAsync(string processCode, bool includeFields = false, CancellationToken cancellationToken = default)
    {
        var query = _context.ProcessDefinitions.AsQueryable();
        if (includeFields)
        {
            query = query.Include(pd => pd.ProcessFields.OrderBy(f => f.SortOrder));
        }
        return await query.FirstOrDefaultAsync(pd => pd.ProcessCode == processCode, cancellationToken);
    }

    public async Task<ProcessDefinition> AddAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default)
    {
        _context.ProcessDefinitions.Add(processDefinition);
        await _context.SaveChangesAsync(cancellationToken);
        return processDefinition;
    }

    public async Task UpdateAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default)
    {
        _context.ProcessDefinitions.Update(processDefinition);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default)
    {
        _context.ProcessDefinitions.Remove(processDefinition);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
