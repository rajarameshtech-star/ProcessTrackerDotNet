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
