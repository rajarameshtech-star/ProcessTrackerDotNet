namespace ProcessTracker.API.Services;
using ProcessTracker.API.DTOs;

public interface IProcessDefinitionProjectMappingService
{
    Task<IEnumerable<ProcessDefinitionProjectMappingDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProcessDefinitionProjectMappingDto>> GetByProcessDefinitionIdAsync(int processDefinitionId, CancellationToken cancellationToken = default);
    Task<ProcessDefinitionProjectMappingDto?> CreateAsync(ProcessDefinitionProjectMappingDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int processDefinitionId, int projectId, CancellationToken cancellationToken = default);
}
