using System.ComponentModel.DataAnnotations;

namespace ProcessTracker.API.DTOs;

public class ProcessDefinitionProjectMappingDto
{
    [Required]
    public int ProcessDefinitionId { get; set; }

    [Required]
    public int ProjectId { get; set; }
}
