namespace ProcessTracker.API.Repositories;
using ProcessTracker.API.Entities;

public interface IProcessRecordRepository
{
    Task<ProcessRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProcessRecord?> GetByServiceItemIdAsync(int serviceItemId, CancellationToken cancellationToken = default);
    Task<ProcessRecord> AddAsync(ProcessRecord processRecord, CancellationToken cancellationToken = default);
    Task UpdateAsync(ProcessRecord processRecord, CancellationToken cancellationToken = default);
    Task DeleteAsync(ProcessRecord processRecord, CancellationToken cancellationToken = default);
}
