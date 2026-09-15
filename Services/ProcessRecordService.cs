using ProcessTracker.API.DTOs;
using ProcessTracker.API.Entities;
using ProcessTracker.API.Repositories;
using ProcessTracker.API.Validators;

namespace ProcessTracker.API.Services;

public class ProcessRecordService : IProcessRecordService
{
    private readonly IProcessRecordRepository _recordRepo;
    private readonly IServiceItemRepository _serviceItemRepo;
    private readonly IProcessDefinitionRepository _processDefRepo;
    private readonly IApplicationRepository _applicationRepo;
    private readonly IProcessDefinitionProjectMappingRepository _mappingRepo;
    private readonly IDynamicProcessValidator _validator;

    public ProcessRecordService(
        IProcessRecordRepository recordRepo,
        IServiceItemRepository serviceItemRepo,
        IProcessDefinitionRepository processDefRepo,
        IApplicationRepository applicationRepo,
        IProcessDefinitionProjectMappingRepository mappingRepo,
        IDynamicProcessValidator validator)
    {
        _recordRepo = recordRepo;
        _serviceItemRepo = serviceItemRepo;
        _processDefRepo = processDefRepo;
        _applicationRepo = applicationRepo;
        _mappingRepo = mappingRepo;
        _validator = validator;
    }

    public async Task<ProcessRecordDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _recordRepo.GetByIdAsync(id, cancellationToken);
        return item == null ? null : MapToDto(item);
    }

    public async Task<ProcessRecordDto?> GetByServiceItemIdAsync(int serviceItemId, CancellationToken cancellationToken = default)
    {
        var item = await _recordRepo.GetByServiceItemIdAsync(serviceItemId, cancellationToken);
        return item == null ? null : MapToDto(item);
    }

    public async Task<(ProcessRecordDto? record, ValidationResult validationResult)> CreateAsync(ProcessRecordDto dto, CancellationToken cancellationToken = default)
    {
        var validationResult = await ValidateCoreAsync(dto.ServiceItemId, dto.ProcessDefinitionId, dto.DataJson, cancellationToken);
        if (!validationResult.IsValid) return (null, validationResult);

        var existingRecord = await _recordRepo.GetByServiceItemIdAsync(dto.ServiceItemId, cancellationToken);
        if (existingRecord != null)
        {
            validationResult.AddError("ServiceItemId", "A ProcessRecord already exists for this ServiceItem.");
            return (null, validationResult);
        }

        var entity = new ProcessRecord
        {
            ServiceItemId = dto.ServiceItemId,
            ProcessDefinitionId = dto.ProcessDefinitionId,
            DataJson = dto.DataJson,
            CreatedDate = dto.CreatedDate != default ? dto.CreatedDate : DateTime.UtcNow,
            CreatedBy = dto.CreatedBy
        };

        var created = await _recordRepo.AddAsync(entity, cancellationToken);
        return (MapToDto(created), validationResult);
    }

    public async Task<(ProcessRecordDto? record, ValidationResult validationResult)> UpdateAsync(int id, ProcessRecordDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _recordRepo.GetByIdAsync(id, cancellationToken);
        if (entity == null) return (null, new ValidationResult());

        if (entity.ServiceItemId != dto.ServiceItemId || entity.ProcessDefinitionId != dto.ProcessDefinitionId)
        {
            var res = new ValidationResult();
            res.AddError("ProcessDefinitionId", "Cannot change ProcessDefinitionId or ServiceItemId on an existing ProcessRecord.");
            return (null, res);
        }

        var validationResult = await ValidateCoreAsync(dto.ServiceItemId, dto.ProcessDefinitionId, dto.DataJson, cancellationToken);
        if (!validationResult.IsValid) return (null, validationResult);

        entity.DataJson = dto.DataJson;
        // Preserve CreatedDate/CreatedBy unless the API strictly updates them (typically preserving is best).
        entity.ModifiedDate = DateTime.UtcNow;
        if (!string.IsNullOrWhiteSpace(dto.ModifiedBy))
            entity.ModifiedBy = dto.ModifiedBy;

        await _recordRepo.UpdateAsync(entity, cancellationToken);
        return (MapToDto(entity), validationResult);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _recordRepo.GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;

        await _recordRepo.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private async Task<ValidationResult> ValidateCoreAsync(int serviceItemId, int processDefinitionId, string dataJson, CancellationToken cancellationToken)
    {
        var result = new ValidationResult();

        var serviceItem = await _serviceItemRepo.GetByIdAsync(serviceItemId, false, cancellationToken);
        if (serviceItem == null)
        {
            result.AddError("ServiceItemId", "ServiceItem not found.");
            return result;
        }

        if (serviceItem.ProcessDefinitionId != processDefinitionId)
        {
            result.AddError("ProcessDefinitionId", "ProcessDefinitionId does not match the ServiceItem's ProcessDefinition.");
            return result;
        }

        var app = await _applicationRepo.GetByIdAsync(serviceItem.ApplicationId, false, cancellationToken);
        if (app == null) 
        {
            result.AddError("ServiceItemId", "Associated Application not found.");
            return result;
        }

        var isMapped = await _mappingRepo.ExistsAsync(processDefinitionId, app.ProjectId, cancellationToken);
        if (!isMapped)
        {
            result.AddError("ProcessDefinitionId", "ProcessDefinition is not mapped to the Application's Project.");
            return result;
        }

        var processDef = await _processDefRepo.GetByIdAsync(processDefinitionId, true, cancellationToken);
        if (processDef == null)
        {
            result.AddError("ProcessDefinitionId", "ProcessDefinition not found.");
            return result;
        }

        var jsonValidation = _validator.Validate(processDef, dataJson);
        foreach(var err in jsonValidation.Errors)
            result.AddError(err.Field, err.Message);

        return result;
    }

    private static ProcessRecordDto MapToDto(ProcessRecord e) => new ProcessRecordDto
    {
        Id = e.Id,
        ServiceItemId = e.ServiceItemId,
        ProcessDefinitionId = e.ProcessDefinitionId,
        DataJson = e.DataJson,
        CreatedDate = e.CreatedDate,
        ModifiedDate = e.ModifiedDate,
        CreatedBy = e.CreatedBy,
        ModifiedBy = e.ModifiedBy
    };
}
