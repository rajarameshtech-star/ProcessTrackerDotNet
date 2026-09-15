using System.ComponentModel.DataAnnotations;

namespace ProcessTracker.API.DTOs;

public class ProcessDefinitionDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string ProcessCode { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string ProcessName { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool IsActive { get; set; }
    public DateTime CreatedDate { get; set; }
}
