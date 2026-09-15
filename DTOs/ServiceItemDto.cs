using System.ComponentModel.DataAnnotations;

namespace ProcessTracker.API.DTOs;

public class ServiceItemDto
{
    public int Id { get; set; }

    [Required]
    public int ApplicationId { get; set; }

    [Required]
    public int ProcessDefinitionId { get; set; }

    [Required]
    [StringLength(50)]
    public string ReferenceNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Status { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    public string Priority { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? AssignedTo { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
