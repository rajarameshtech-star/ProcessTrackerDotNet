namespace ProcessTracker.API.Services;
using ProcessTracker.API.DTOs;

public interface IProcessDefinitionService
{
    Task<IEnumerable<ProcessDefinitionDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProcessDefinitionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProcessDefinitionDto?> CreateAsync(ProcessDefinitionDto dto, CancellationToken cancellationToken = default);
    Task<ProcessDefinitionDto?> UpdateAsync(int id, ProcessDefinitionDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
