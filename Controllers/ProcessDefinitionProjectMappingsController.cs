using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessTracker.API.DTOs;
using ProcessTracker.API.Services;

namespace ProcessTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessDefinitionProjectMappingsController : ControllerBase
{
    private readonly IProcessDefinitionProjectMappingService _service;

    public ProcessDefinitionProjectMappingsController(IProcessDefinitionProjectMappingService service)
    {
        _service = service;
    }

    [HttpGet("by-project/{projectId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProcessDefinitionProjectMappingDto>))]
    public async Task<ActionResult<IEnumerable<ProcessDefinitionProjectMappingDto>>> GetByProject(int projectId)
    {
        var items = await _service.GetByProjectIdAsync(projectId);
        return Ok(items);
    }

    [HttpGet("by-process-definition/{processDefinitionId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProcessDefinitionProjectMappingDto>))]
    public async Task<ActionResult<IEnumerable<ProcessDefinitionProjectMappingDto>>> GetByProcessDefinition(int processDefinitionId)
    {
        var items = await _service.GetByProcessDefinitionIdAsync(processDefinitionId);
        return Ok(items);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessDefinitionProjectMappingDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProcessDefinitionProjectMappingDto>> Create(ProcessDefinitionProjectMappingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(dto);
        if (created == null) return BadRequest("Project or ProcessDefinition not found.");
        return Ok(created); // Typically 201 with location, but no single Get exists for composite
    }

    [HttpDelete("{processDefinitionId}/{projectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int processDefinitionId, int projectId)
    {
        var result = await _service.DeleteAsync(processDefinitionId, projectId);
        if (!result) return NotFound();
        return Ok();
    }
}
