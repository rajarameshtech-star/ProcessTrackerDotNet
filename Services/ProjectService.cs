using ProcessTracker.API.DTOs;
using ProcessTracker.API.Entities;
using ProcessTracker.API.Repositories;

namespace ProcessTracker.API.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;

    public ProjectService(IProjectRepository projectRepository)
    {
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var projects = await _projectRepository.GetAllAsync(cancellationToken);
        return projects.Select(MapToDto);
    }

    public async Task<ProjectDto?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, includeDetails, cancellationToken);
        return project == null ? null : MapToDto(project);
    }

    public async Task<ProjectDto> CreateAsync(ProjectDto projectDto, CancellationToken cancellationToken = default)
    {
        var project = new Project
        {
            Name = projectDto.Name,
            Description = projectDto.Description
        };

        var created = await _projectRepository.AddAsync(project, cancellationToken);
        return MapToDto(created);
    }

    public async Task<ProjectDto?> UpdateAsync(int id, ProjectDto projectDto, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, false, cancellationToken);
        if (project == null) return null;

        project.Name = projectDto.Name;
        project.Description = projectDto.Description;

        await _projectRepository.UpdateAsync(project, cancellationToken);
        return MapToDto(project);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(id, true, cancellationToken);
        if (project == null) return false;

        // Simple check to prevent direct cascade issues if any specific rules are required.
        // EF will handle cascade deletes if properly mapped, but we just pass it down safely.
        await _projectRepository.DeleteAsync(project, cancellationToken);
        return true;
    }

    private static ProjectDto MapToDto(Project p) => new ProjectDto
    {
        Id = p.Id,
        Name = p.Name,
        Description = p.Description
    };
}
