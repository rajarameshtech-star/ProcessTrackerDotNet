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
    public async Task<IActionResult> GetByProject(int projectId)
    {
        var items = await _service.GetByProjectIdAsync(projectId);
        return Ok(items);
    }

    [HttpGet("by-process-definition/{processDefinitionId}")]
    public async Task<IActionResult> GetByProcessDefinition(int processDefinitionId)
    {
        var items = await _service.GetByProcessDefinitionIdAsync(processDefinitionId);
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProcessDefinitionProjectMappingDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = await _service.CreateAsync(dto);
            if (created == null) return BadRequest("Project or ProcessDefinition not found.");
            return Ok(created); // Typically 201 with location, but no single Get exists for composite
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }

    [HttpDelete("{processDefinitionId}/{projectId}")]
    public async Task<IActionResult> Delete(int processDefinitionId, int projectId)
    {
        var result = await _service.DeleteAsync(processDefinitionId, projectId);
        if (!result) return NotFound();
        return Ok();
    }
}
