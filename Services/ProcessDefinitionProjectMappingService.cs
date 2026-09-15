using ProcessTracker.API.DTOs;
using ProcessTracker.API.Entities;
using ProcessTracker.API.Repositories;

namespace ProcessTracker.API.Services;

public class ProcessDefinitionProjectMappingService : IProcessDefinitionProjectMappingService
{
    private readonly IProcessDefinitionProjectMappingRepository _mappingRepo;
    private readonly IProcessDefinitionRepository _processDefRepo;
    private readonly IProjectRepository _projectRepo;

    public ProcessDefinitionProjectMappingService(
        IProcessDefinitionProjectMappingRepository mappingRepo,
        IProcessDefinitionRepository processDefRepo,
        IProjectRepository projectRepo)
    {
        _mappingRepo = mappingRepo;
        _processDefRepo = processDefRepo;
        _projectRepo = projectRepo;
    }

    public async Task<IEnumerable<ProcessDefinitionProjectMappingDto>> GetByProjectIdAsync(int projectId, CancellationToken cancellationToken = default)
    {
        var items = await _mappingRepo.GetByProjectIdAsync(projectId, cancellationToken);
        return items.Select(MapToDto);
    }

    public async Task<IEnumerable<ProcessDefinitionProjectMappingDto>> GetByProcessDefinitionIdAsync(int processDefinitionId, CancellationToken cancellationToken = default)
    {
        var items = await _mappingRepo.GetByProcessDefinitionIdAsync(processDefinitionId, cancellationToken);
        return items.Select(MapToDto);
    }

    public async Task<ProcessDefinitionProjectMappingDto?> CreateAsync(ProcessDefinitionProjectMappingDto dto, CancellationToken cancellationToken = default)
    {
        if (await _projectRepo.GetByIdAsync(dto.ProjectId, false, cancellationToken) == null)
            return null;
        if (await _processDefRepo.GetByIdAsync(dto.ProcessDefinitionId, false, cancellationToken) == null)
            return null;

        if (await _mappingRepo.ExistsAsync(dto.ProcessDefinitionId, dto.ProjectId, cancellationToken))
            throw new InvalidOperationException("Mapping already exists.");

        var entity = new ProcessDefinitionProjectMapping
        {
            ProcessDefinitionId = dto.ProcessDefinitionId,
            ProjectId = dto.ProjectId
        };

        var created = await _mappingRepo.AddAsync(entity, cancellationToken);
        return MapToDto(created);
    }

    public async Task<bool> DeleteAsync(int processDefinitionId, int projectId, CancellationToken cancellationToken = default)
    {
        var mapping = await _mappingRepo.GetAsync(processDefinitionId, projectId, cancellationToken);
        if (mapping == null) return false;

        await _mappingRepo.DeleteAsync(mapping, cancellationToken);
        return true;
    }

    private static ProcessDefinitionProjectMappingDto MapToDto(ProcessDefinitionProjectMapping e) => new ProcessDefinitionProjectMappingDto
    {
        ProcessDefinitionId = e.ProcessDefinitionId,
        ProjectId = e.ProjectId
    };
}
