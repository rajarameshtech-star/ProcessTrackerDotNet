using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Repositories;

public class ProcessFieldRepository : IProcessFieldRepository
{
    private readonly ProcessTrackerDbContext _context;

    public ProcessFieldRepository(ProcessTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ProcessField>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ProcessFields.AsNoTracking().OrderBy(f => f.ProcessDefinitionId).ThenBy(f => f.SortOrder).ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ProcessField>> GetByProcessDefinitionIdAsync(int processDefinitionId, bool activeOnly = false, CancellationToken cancellationToken = default)
    {
        var query = _context.ProcessFields.AsNoTracking().Where(f => f.ProcessDefinitionId == processDefinitionId);
        if (activeOnly)
        {
            query = query.Where(f => f.IsActive);
        }
        return await query.OrderBy(f => f.SortOrder).ToListAsync(cancellationToken);
    }

    public async Task<ProcessField?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.ProcessFields.FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
    }

    public async Task<ProcessField> AddAsync(ProcessField processField, CancellationToken cancellationToken = default)
    {
        _context.ProcessFields.Add(processField);
        await _context.SaveChangesAsync(cancellationToken);
        return processField;
    }

    public async Task UpdateAsync(ProcessField processField, CancellationToken cancellationToken = default)
    {
        _context.ProcessFields.Update(processField);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ProcessField processField, CancellationToken cancellationToken = default)
    {
        _context.ProcessFields.Remove(processField);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
