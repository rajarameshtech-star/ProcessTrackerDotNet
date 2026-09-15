using System.ComponentModel.DataAnnotations;

namespace ProcessTracker.API.DTOs;

public class ApplicationDto
{
    public int Id { get; set; }

    [Required]
    public int ProjectId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;
}
