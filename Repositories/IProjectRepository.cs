namespace ProcessTracker.API.Repositories;
using ProcessTracker.API.Entities;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Project?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<Project> AddAsync(Project project, CancellationToken cancellationToken = default);
    Task UpdateAsync(Project project, CancellationToken cancellationToken = default);
    Task DeleteAsync(Project project, CancellationToken cancellationToken = default);
}
