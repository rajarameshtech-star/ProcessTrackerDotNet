namespace ProcessTracker.API.Services;
using ProcessTracker.API.DTOs;
using ProcessTracker.API.Validators;

public interface IProcessRecordService
{
    Task<ProcessRecordDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<ProcessRecordDto?> GetByServiceItemIdAsync(int serviceItemId, CancellationToken cancellationToken = default);
    Task<(ProcessRecordDto? record, ValidationResult validationResult)> CreateAsync(ProcessRecordDto dto, CancellationToken cancellationToken = default);
    Task<(ProcessRecordDto? record, ValidationResult validationResult)> UpdateAsync(int id, ProcessRecordDto dto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
