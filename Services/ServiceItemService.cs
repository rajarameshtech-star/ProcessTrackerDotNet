using ProcessTracker.API.DTOs;
using ProcessTracker.API.Entities;
using ProcessTracker.API.Repositories;

namespace ProcessTracker.API.Services;

public class ServiceItemService : IServiceItemService
{
    private readonly IServiceItemRepository _serviceItemRepo;
    private readonly IApplicationRepository _applicationRepo;
    private readonly IProcessDefinitionRepository _processDefRepo;
    private readonly IProcessDefinitionProjectMappingRepository _mappingRepo;
    private readonly IProcessRecordRepository _recordRepo;

    public ServiceItemService(
        IServiceItemRepository serviceItemRepo,
        IApplicationRepository applicationRepo,
        IProcessDefinitionRepository processDefRepo,
        IProcessDefinitionProjectMappingRepository mappingRepo,
        IProcessRecordRepository recordRepo)
    {
        _serviceItemRepo = serviceItemRepo;
        _applicationRepo = applicationRepo;
        _processDefRepo = processDefRepo;
        _mappingRepo = mappingRepo;
        _recordRepo = recordRepo;
    }

    public async Task<IEnumerable<ServiceItemDto>> GetAllAsync(int? applicationId = null, CancellationToken cancellationToken = default)
    {
        var items = applicationId.HasValue
            ? await _serviceItemRepo.GetByApplicationIdAsync(applicationId.Value, cancellationToken)
            : await _serviceItemRepo.GetAllAsync(cancellationToken);
        
        return items.Select(MapToDto);
    }

    public async Task<ServiceItemDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _serviceItemRepo.GetByIdAsync(id, false, cancellationToken);
        return item == null ? null : MapToDto(item);
    }

    public async Task<ServiceItemDto?> CreateAsync(ServiceItemDto dto, CancellationToken cancellationToken = default)
    {
        await ValidateCompatibilityAsync(dto.ApplicationId, dto.ProcessDefinitionId, cancellationToken);

        var entity = new ServiceItem
        {
            ApplicationId = dto.ApplicationId,
            ProcessDefinitionId = dto.ProcessDefinitionId,
            ReferenceNumber = dto.ReferenceNumber,
            Title = dto.Title,
            Status = dto.Status,
            Priority = dto.Priority,
            AssignedTo = dto.AssignedTo,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _serviceItemRepo.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    public async Task<ServiceItemDto?> UpdateAsync(int id, ServiceItemDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _serviceItemRepo.GetByIdAsync(id, false, cancellationToken);
        if (entity == null) return null;

        await ValidateCompatibilityAsync(dto.ApplicationId, dto.ProcessDefinitionId, cancellationToken);

        if (entity.ProcessDefinitionId != dto.ProcessDefinitionId)
        {
            var record = await _recordRepo.GetByServiceItemIdAsync(id, cancellationToken);
            if (record != null)
                throw new InvalidOperationException("Cannot change ProcessDefinitionId when a ProcessRecord already exists for this ServiceItem.");
        }

        entity.ApplicationId = dto.ApplicationId;
        entity.ProcessDefinitionId = dto.ProcessDefinitionId;
        entity.ReferenceNumber = dto.ReferenceNumber;
        entity.Title = dto.Title;
        entity.Status = dto.Status;
        entity.Priority = dto.Priority;
        entity.AssignedTo = dto.AssignedTo;
        entity.UpdatedAt = DateTime.UtcNow;

        await _serviceItemRepo.UpdateAsync(entity, cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _serviceItemRepo.GetByIdAsync(id, false, cancellationToken);
        if (entity == null) return false;

        await _serviceItemRepo.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private async Task ValidateCompatibilityAsync(int applicationId, int processDefinitionId, CancellationToken cancellationToken)
    {
        var app = await _applicationRepo.GetByIdAsync(applicationId, false, cancellationToken);
        if (app == null) throw new ArgumentException($"Application {applicationId} not found.");

        var processDef = await _processDefRepo.GetByIdAsync(processDefinitionId, false, cancellationToken);
        if (processDef == null) throw new ArgumentException($"ProcessDefinition {processDefinitionId} not found.");

        var isMapped = await _mappingRepo.ExistsAsync(processDefinitionId, app.ProjectId, cancellationToken);
        if (!isMapped)
            throw new InvalidOperationException("ProcessDefinition is not mapped to the Application's Project.");
    }

    private static ServiceItemDto MapToDto(ServiceItem e) => new ServiceItemDto
    {
        Id = e.Id,
        ApplicationId = e.ApplicationId,
        ProcessDefinitionId = e.ProcessDefinitionId,
        ReferenceNumber = e.ReferenceNumber,
        Title = e.Title,
        Status = e.Status,
        Priority = e.Priority,
        AssignedTo = e.AssignedTo,
        CreatedAt = e.CreatedAt,
        UpdatedAt = e.UpdatedAt
    };
}
