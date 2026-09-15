using Microsoft.EntityFrameworkCore;
using ProcessTracker.API.Data;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.Repositories;

public class ApplicationRepository : IApplicationRepository
{
    private readonly ProcessTrackerDbContext _context;

    public ApplicationRepository(ProcessTrackerDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Application>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Applications.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Application>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        return await _context.Applications
            .AsNoTracking()
            .Where(a => a.ProjectId == projectId)
            .ToListAsync(cancellationToken);
    }

    public async Task<Application?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var query = _context.Applications.AsQueryable();

        if (includeDetails)
        {
            query = query.Include(a => a.Project)
                         .Include(a => a.ServiceItems);
        }

        return await query.FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    public async Task<Application> AddAsync(Application application, CancellationToken cancellationToken = default)
    {
        _context.Applications.Add(application);
        await _context.SaveChangesAsync(cancellationToken);
        return application;
    }

    public async Task UpdateAsync(Application application, CancellationToken cancellationToken = default)
    {
        _context.Applications.Update(application);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Application application, CancellationToken cancellationToken = default)
    {
        _context.Applications.Remove(application);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
