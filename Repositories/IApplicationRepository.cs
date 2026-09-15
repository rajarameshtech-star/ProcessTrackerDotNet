namespace ProcessTracker.API.Repositories;
using ProcessTracker.API.Entities;

public interface IApplicationRepository
{
    Task<IEnumerable<Application>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<Application>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<Application?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<Application> AddAsync(Application application, CancellationToken cancellationToken = default);
    Task UpdateAsync(Application application, CancellationToken cancellationToken = default);
    Task DeleteAsync(Application application, CancellationToken cancellationToken = default);
}
