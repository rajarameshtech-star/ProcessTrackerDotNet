using System.ComponentModel.DataAnnotations;
using ProcessTracker.API.Entities;

namespace ProcessTracker.API.DTOs;

public class ProcessFieldDto
{
    public int Id { get; set; }

    [Required]
    public int ProcessDefinitionId { get; set; }

    [Required]
    [StringLength(50)]
    public string FieldName { get; set; } = string.Empty;

    [Required]
    [StringLength(100)]
    public string Label { get; set; } = string.Empty;

    [Required]
    public FieldType FieldType { get; set; }

    public bool IsRequired { get; set; }
    public int SortOrder { get; set; }

    [MaxLength(200)]
    public string? Placeholder { get; set; }

    [MaxLength(200)]
    public string? DefaultValue { get; set; }

    public string? OptionsJson { get; set; }
    public int? MinLength { get; set; }
    public int? MaxLength { get; set; }
    public bool IsActive { get; set; }
}
