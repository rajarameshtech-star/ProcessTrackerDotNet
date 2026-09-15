namespace ProcessTracker.API.Services;
using ProcessTracker.API.DTOs;

public interface IApplicationService
{
    Task<IEnumerable<ApplicationDto>> GetAllAsync(int? projectId = null, CancellationToken cancellationToken = default);
    Task<ApplicationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ApplicationDto?> CreateAsync(ApplicationDto applicationDto, CancellationToken cancellationToken = default);
    Task<ApplicationDto?> UpdateAsync(int id, ApplicationDto applicationDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
