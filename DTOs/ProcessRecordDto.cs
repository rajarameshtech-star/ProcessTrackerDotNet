using System.ComponentModel.DataAnnotations;

namespace ProcessTracker.API.DTOs;

public class ProcessRecordDto
{
    public int Id { get; set; }

    [Required]
    public int ServiceItemId { get; set; }

    [Required]
    public int ProcessDefinitionId { get; set; }

    [Required]
    [MinLength(1)]
    public string DataJson { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }

    [Required]
    [StringLength(100)]
    public string CreatedBy { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ModifiedBy { get; set; }
}
