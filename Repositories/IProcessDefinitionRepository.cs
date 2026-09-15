namespace ProcessTracker.API.Repositories;
using ProcessTracker.API.Entities;

public interface IProcessDefinitionRepository
{
    Task<IEnumerable<ProcessDefinition>> GetAllAsync(bool activeOnly = false, CancellationToken cancellationToken = default);
    Task<ProcessDefinition?> GetByIdAsync(int id, bool includeFields = false, CancellationToken cancellationToken = default);
    Task<ProcessDefinition?> GetByCodeAsync(string processCode, bool includeFields = false, CancellationToken cancellationToken = default);
    Task<ProcessDefinition> AddAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default);
    Task DeleteAsync(ProcessDefinition processDefinition, CancellationToken cancellationToken = default);
}
