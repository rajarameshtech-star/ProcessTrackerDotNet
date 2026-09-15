using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessTracker.API.DTOs;
using ProcessTracker.API.Services;

namespace ProcessTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessFieldsController : ControllerBase
{
    private readonly IProcessFieldService _service;

    public ProcessFieldsController(IProcessFieldService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ProcessFieldDto>))]
    public async Task<ActionResult<IEnumerable<ProcessFieldDto>>> GetAll([FromQuery] int? processDefinitionId)
    {
        var items = await _service.GetAllAsync(processDefinitionId);
        return Ok(items);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessFieldDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessFieldDto>> GetById(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item == null) return NotFound();
        return Ok(item);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProcessFieldDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProcessFieldDto>> Create(ProcessFieldDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        try
        {
            var created = await _service.CreateAsync(dto);
            if (created == null) return BadRequest(new { message = "Associated ProcessDefinition not found." });
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessFieldDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessFieldDto>> Update(int id, ProcessFieldDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (id != dto.Id) return BadRequest();
        try
        {
            var updated = await _service.UpdateAsync(id, dto);
            if (updated == null) return NotFound();
            return Ok(updated);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok();
    }
}
