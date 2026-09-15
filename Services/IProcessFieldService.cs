namespace ProcessTracker.API.Services;
using ProcessTracker.API.DTOs;

public interface IProcessFieldService
{
    Task<IEnumerable<ProcessFieldDto>> GetAllAsync(int? processDefinitionId = null, CancellationToken cancellationToken = default);
    Task<ProcessFieldDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProcessFieldDto?> CreateAsync(ProcessFieldDto dto, CancellationToken cancellationToken = default);
    Task<ProcessFieldDto?> UpdateAsync(int id, ProcessFieldDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
