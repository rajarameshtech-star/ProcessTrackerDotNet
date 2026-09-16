using ProcessTracker.API.DTOs;
using ProcessTracker.API.Entities;
using ProcessTracker.API.Repositories;
using ProcessTracker.API.Exceptions;

namespace ProcessTracker.API.Services;

public class ApplicationService : IApplicationService
{
    private readonly IApplicationRepository _applicationRepository;
    private readonly IProjectRepository _projectRepository;

    public ApplicationService(IApplicationRepository applicationRepository, IProjectRepository projectRepository)
    {
        _applicationRepository = applicationRepository;
        _projectRepository = projectRepository;
    }

    public async Task<IEnumerable<ApplicationDto>> GetAllAsync(int? projectId = null, CancellationToken cancellationToken = default)
    {
        var apps = projectId.HasValue
            ? await _applicationRepository.GetByProjectIdAsync(projectId.Value, cancellationToken)
            : await _applicationRepository.GetAllAsync(cancellationToken);
        
        return apps.Select(MapToDto);
    }

    public async Task<ApplicationDto?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var app = await _applicationRepository.GetByIdAsync(id, false, cancellationToken);
        return app == null ? null : MapToDto(app);
    }

    public async Task<ApplicationDto?> CreateAsync(ApplicationDto applicationDto, CancellationToken cancellationToken = default)
    {
        var project = await _projectRepository.GetByIdAsync(applicationDto.ProjectId, false, cancellationToken);
        if (project == null) return null; // Project must exist

        var app = new Application
        {
            ProjectId = applicationDto.ProjectId,
            Name = applicationDto.Name,
            Description = applicationDto.Description
        };

        var created = await _applicationRepository.AddAsync(app, cancellationToken);
        return MapToDto(created);
    }

    public async Task<ApplicationDto?> UpdateAsync(int id, ApplicationDto applicationDto, CancellationToken cancellationToken = default)
    {
        var app = await _applicationRepository.GetByIdAsync(id, false, cancellationToken);
        if (app == null) return null;

        var project = await _projectRepository.GetByIdAsync(applicationDto.ProjectId, false, cancellationToken);
        if (project == null) return null; // Must still reference valid project

        app.ProjectId = applicationDto.ProjectId;
        app.Name = applicationDto.Name;
        app.Description = applicationDto.Description;

        await _applicationRepository.UpdateAsync(app, cancellationToken);
        return MapToDto(app);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var app = await _applicationRepository.GetByIdAsync(id, true, cancellationToken);
        if (app == null) return false;

        if (app.ServiceItems.Any()) 
            throw new ProcessTrackerConflictException("Cannot delete application because it contains ServiceItems.");

        await _applicationRepository.DeleteAsync(app, cancellationToken);
        return true;
    }

    private static ApplicationDto MapToDto(Application a) => new ApplicationDto
    {
        Id = a.Id,
        ProjectId = a.ProjectId,
        Name = a.Name,
        Description = a.Description
    };
}
