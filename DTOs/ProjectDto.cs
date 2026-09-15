using System.ComponentModel.DataAnnotations;

namespace ProcessTracker.API.DTOs;

public class ProjectDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}
