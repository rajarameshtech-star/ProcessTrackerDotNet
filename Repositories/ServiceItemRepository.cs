using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Repositories;

public class ServiceItemRepository : IServiceItemRepository
{
    private readonly ProcessTrackerDbContext _context;

    public ServiceItemRepository(ProcessTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<ServiceItem>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.ServiceItems.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<ServiceItem> Items, int TotalCount)> GetPagedAsync(
        int? applicationId = null,
        int? processDefinitionId = null,
        string? status = null,
        string? priority = null,
        int pageNumber = 1,
        int pageSize = 10,
        int? projectId = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.ServiceItems.AsNoTracking().AsQueryable();

        if (applicationId.HasValue)
            query = query.Where(s => s.ApplicationId == applicationId.Value);
        else if (projectId.HasValue)
            query = query.Where(s => s.Application != null && s.Application.ProjectId == projectId.Value);

        if (processDefinitionId.HasValue)
            query = query.Where(s => s.ProcessDefinitionId == processDefinitionId.Value);

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.Status == status);

        if (!string.IsNullOrWhiteSpace(priority))
            query = query.Where(s => s.Priority == priority);

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<ServiceItem>> GetByApplicationIdAsync(int applicationId, CancellationToken cancellationToken = default)
    {
        return await _context.ServiceItems
            .AsNoTracking()
            .Where(s => s.ApplicationId == applicationId)
            .ToListAsync(cancellationToken);
    }

    public async Task<ServiceItem?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = _context.ServiceItems.AsQueryable();

        if (includeDetails)
        {
            query = query.Include(s => s.Application)
                         .Include(s => s.ProcessDefinition)
                         .Include(s => s.ProcessRecord);
        }

        return await query.FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<ServiceItem> AddAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default)
    {
        _context.ServiceItems.Add(serviceItem);
        await _context.SaveChangesAsync(cancellationToken);
        return serviceItem;
    }

    public async Task UpdateAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default)
    {
        _context.ServiceItems.Update(serviceItem);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default)
    {
        _context.ServiceItems.Remove(serviceItem);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
