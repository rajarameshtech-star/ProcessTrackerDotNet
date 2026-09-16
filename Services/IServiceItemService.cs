namespace ProcessTracker.API.Services;
using ProcessTracker.API.DTOs;

public interface IServiceItemService
{
    Task<PagedResult<ServiceItemDto>> GetAllAsync(
        int? applicationId = null, 
        int? processDefinitionId = null, 
        string? status = null, 
        string? priority = null, 
        int pageNumber = 1, 
        int pageSize = 10,
        int? projectId = null,
        CancellationToken cancellationToken = default);
    Task<ServiceItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceItemDto?> CreateAsync(ServiceItemDto dto, CancellationToken cancellationToken = default);
    Task<ServiceItemDto?> UpdateAsync(int id, ServiceItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
