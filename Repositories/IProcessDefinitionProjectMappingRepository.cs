namespace ProcessTracker.API.Repositories;
using ProcessTracker.API.Entities;

public interface IProcessDefinitionProjectMappingRepository
{
    Task<IEnumerable<ProcessDefinitionProjectMapping>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ProcessDefinitionProjectMapping>> GetByProcessDefinitionIdAsync(int processDefinitionId, CancellationToken cancellationToken = default);
    Task<ProcessDefinitionProjectMapping?> GetAsync(int processDefinitionId, int projectId, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(int processDefinitionId, int projectId, CancellationToken cancellationToken = default);
    Task<ProcessDefinitionProjectMapping> AddAsync(ProcessDefinitionProjectMapping mapping, CancellationToken cancellationToken = default);
    Task DeleteAsync(ProcessDefinitionProjectMapping mapping, CancellationToken cancellationToken = default);
}
