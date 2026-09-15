namespace ProcessTracker.API.Services;
using ProcessTracker.API.DTOs;

public interface IProjectService
{
    Task<IEnumerable<ProjectDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<ProjectDto?> GetByIdAsync(int id, bool includeDetails = false, CancellationToken cancellationToken = default);
    Task<ProjectDto> CreateAsync(ProjectDto projectDto, CancellationToken cancellationToken = default);
    Task<ProjectDto?> UpdateAsync(int id, ProjectDto projectDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
