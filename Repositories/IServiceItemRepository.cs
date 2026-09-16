namespace ProcessTracker.API.Repositories;
using ProcessTracker.API.Entities;

public interface IServiceItemRepository
{
    Task<IEnumerable<ServiceItem>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<(IEnumerable<ServiceItem> Items, int TotalCount)> GetPagedAsync(
        int? applicationId = null,
        int? processDefinitionId = null,
        string? status = null,
        string? priority = null,
        int pageNumber = 1,
        int pageSize = 10,
        int? projectId = null,
        CancellationToken cancellationToken = default);
    Task<IEnumerable<ServiceItem>> GetByApplicationIdAsync(int applicationId, CancellationToken cancellationToken = default);
    Task<ServiceItem?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<ServiceItem> AddAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default);
    Task UpdateAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default);
    Task DeleteAsync(ServiceItem serviceItem, CancellationToken cancellationToken = default);
}
