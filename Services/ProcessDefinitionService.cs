using ProcessTracker.API.DTOs;
using ProcessTracker.API.Entities;
using ProcessTracker.API.Repositories;

namespace ProcessTracker.API.Services;

public class ProcessDefinitionService : IProcessDefinitionService
{
    private readonly IProcessDefinitionRepository _processDefRepo;

    public ProcessDefinitionService(IProcessDefinitionRepository processDefRepo)
    {
        _processDefRepo = processDefRepo;
    }

    public async Task<IEnumerable<ProcessDefinitionDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var items = await _processDefRepo.GetAllAsync(false, cancellationToken);
        return items.Select(MapToDto);
    }

    public async Task<ProcessDefinitionDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _processDefRepo.GetByIdAsync(id, false, cancellationToken);
        return item == null ? null : MapToDto(item);
    }

    public async Task<ProcessDefinitionDto?> CreateAsync(ProcessDefinitionDto dto, CancellationToken cancellationToken = default)
    {
        var existing = await _processDefRepo.GetByCodeAsync(dto.ProcessCode, false, cancellationToken);
        if (existing != null) throw new InvalidOperationException($"ProcessCode '{dto.ProcessCode}' already exists.");

        var entity = new ProcessDefinition
        {
            ProcessCode = dto.ProcessCode,
            ProcessName = dto.ProcessName,
            Description = dto.Description,
            IsActive = dto.IsActive,
            CreatedDate = DateTime.UtcNow
        };

        var created = await _processDefRepo.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    public async Task<ProcessDefinitionDto?> UpdateAsync(int id, ProcessDefinitionDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _processDefRepo.GetByIdAsync(id, false, cancellationToken);
        if (entity == null) return null;

        if (entity.ProcessCode != dto.ProcessCode)
        {
            var existing = await _processDefRepo.GetByCodeAsync(dto.ProcessCode, false, cancellationToken);
            if (existing != null) throw new InvalidOperationException($"ProcessCode '{dto.ProcessCode}' already exists.");
        }

        entity.ProcessCode = dto.ProcessCode;
        entity.ProcessName = dto.ProcessName;
        entity.Description = dto.Description;
        entity.IsActive = dto.IsActive;

        await _processDefRepo.UpdateAsync(entity, cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _processDefRepo.GetByIdAsync(id, true, cancellationToken);
        if (entity == null) return false;

        if (entity.ProcessFields.Any() || entity.ServiceItems.Any() || entity.ProcessRecords.Any() || entity.ProcessDefinitionProjectMappings.Any())
        {
            throw new InvalidOperationException("Cannot delete ProcessDefinition because it has dependent records.");
        }

        await _processDefRepo.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static ProcessDefinitionDto MapToDto(ProcessDefinition e) => new ProcessDefinitionDto
    {
        Id = e.Id,
        ProcessCode = e.ProcessCode,
        ProcessName = e.ProcessName,
        Description = e.Description,
        IsActive = e.IsActive,
        CreatedDate = e.CreatedDate
    };
}
