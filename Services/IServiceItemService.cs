namespace ProcessTracker.API.Services;
using ProcessTracker.API.DTOs;

public interface IServiceItemService
{
    Task<IEnumerable<ServiceItemDto>> GetAllAsync(int? applicationId = null, CancellationToken cancellationToken = default);
    Task<ServiceItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ServiceItemDto?> CreateAsync(ServiceItemDto dto, CancellationToken cancellationToken = default);
    Task<ServiceItemDto?> UpdateAsync(int id, ServiceItemDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
