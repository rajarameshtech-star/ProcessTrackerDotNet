using System.Text.Json;
using ProcessTracker.API.DTOs;
using ProcessTracker.API.Entities;
using ProcessTracker.API.Repositories;

namespace ProcessTracker.API.Services;

public class ProcessFieldService : IProcessFieldService
{
    private readonly IProcessFieldRepository _fieldRepo;
    private readonly IProcessDefinitionRepository _defRepo;

    public ProcessFieldService(IProcessFieldRepository fieldRepo, IProcessDefinitionRepository defRepo)
    {
        _fieldRepo = fieldRepo;
        _defRepo = defRepo;
    }

    public async Task<IEnumerable<ProcessFieldDto>> GetAllAsync(int? processDefinitionId = null, CancellationToken cancellationToken = default)
    {
        var items = processDefinitionId.HasValue
            ? await _fieldRepo.GetByProcessDefinitionIdAsync(processDefinitionId.Value, false, cancellationToken)
            : await _fieldRepo.GetAllAsync(cancellationToken);
        
        return items.Select(MapToDto);
    }

    public async Task<ProcessFieldDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var item = await _fieldRepo.GetByIdAsync(id, cancellationToken);
        return item == null ? null : MapToDto(item);
    }

    public async Task<ProcessFieldDto?> CreateAsync(ProcessFieldDto dto, CancellationToken cancellationToken = default)
    {
        if (await _defRepo.GetByIdAsync(dto.ProcessDefinitionId, false, cancellationToken) == null)
            return null; // Parent ProcessDefinition must exist

        ValidateMetadata(dto);

        var entity = new ProcessField
        {
            ProcessDefinitionId = dto.ProcessDefinitionId,
            FieldName = dto.FieldName,
            Label = dto.Label,
            FieldType = dto.FieldType,
            IsRequired = dto.IsRequired,
            SortOrder = dto.SortOrder,
            Placeholder = dto.Placeholder,
            DefaultValue = dto.DefaultValue,
            OptionsJson = dto.OptionsJson,
            MinLength = dto.MinLength,
            MaxLength = dto.MaxLength,
            IsActive = dto.IsActive
        };

        var created = await _fieldRepo.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    public async Task<ProcessFieldDto?> UpdateAsync(int id, ProcessFieldDto dto, CancellationToken cancellationToken = default)
    {
        var entity = await _fieldRepo.GetByIdAsync(id, cancellationToken);
        if (entity == null) return null;

        if (await _defRepo.GetByIdAsync(dto.ProcessDefinitionId, false, cancellationToken) == null)
            return null;

        ValidateMetadata(dto);

        entity.ProcessDefinitionId = dto.ProcessDefinitionId;
        entity.FieldName = dto.FieldName;
        entity.Label = dto.Label;
        entity.FieldType = dto.FieldType;
        entity.IsRequired = dto.IsRequired;
        entity.SortOrder = dto.SortOrder;
        entity.Placeholder = dto.Placeholder;
        entity.DefaultValue = dto.DefaultValue;
        entity.OptionsJson = dto.OptionsJson;
        entity.MinLength = dto.MinLength;
        entity.MaxLength = dto.MaxLength;
        entity.IsActive = dto.IsActive;

        await _fieldRepo.UpdateAsync(entity, cancellationToken);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _fieldRepo.GetByIdAsync(id, cancellationToken);
        if (entity == null) return false;

        await _fieldRepo.DeleteAsync(entity, cancellationToken);
        return true;
    }

    private static void ValidateMetadata(ProcessFieldDto dto)
    {
        if (dto.MinLength.HasValue && dto.MaxLength.HasValue && dto.MinLength > dto.MaxLength)
            throw new InvalidOperationException("MinLength cannot be greater than MaxLength.");
        
        if (dto.FieldType == FieldType.Select)
        {
            if (string.IsNullOrWhiteSpace(dto.OptionsJson))
                throw new InvalidOperationException("OptionsJson is required for Select FieldType.");
            try
            {
                var options = JsonSerializer.Deserialize<List<string>>(dto.OptionsJson);
                if (options == null || options.Count == 0)
                    throw new InvalidOperationException("OptionsJson must contain at least one option.");
            }
            catch
            {
                throw new InvalidOperationException("OptionsJson must be a valid JSON array of strings.");
            }
        }
    }

    private static ProcessFieldDto MapToDto(ProcessField e) => new ProcessFieldDto
    {
        Id = e.Id,
        ProcessDefinitionId = e.ProcessDefinitionId,
        FieldName = e.FieldName,
        Label = e.Label,
        FieldType = e.FieldType,
        IsRequired = e.IsRequired,
        SortOrder = e.SortOrder,
        Placeholder = e.Placeholder,
        DefaultValue = e.DefaultValue,
        OptionsJson = e.OptionsJson,
        MinLength = e.MinLength,
        MaxLength = e.MaxLength,
        IsActive = e.IsActive
    };
}
