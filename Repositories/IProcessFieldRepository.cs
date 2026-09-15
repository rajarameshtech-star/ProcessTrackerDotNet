namespace ProcessTracker.API.Repositories;
using ProcessTracker.API.Entities;

public interface IProcessFieldRepository
{
    Task<IEnumerable<ProcessField>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<ProcessField>> GetByProcessDefinitionIdAsync(int processDefinitionId, bool activeOnly = false, CancellationToken cancellationToken = default);
    Task<ProcessField?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProcessField> AddAsync(ProcessField processField, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProcessField processField, CancellationToken cancellationToken = default);
    Task DeleteAsync(ProcessField processField, CancellationToken cancellationToken = default);
}
