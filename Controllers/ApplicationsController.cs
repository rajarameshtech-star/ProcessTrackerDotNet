using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessTracker.API.DTOs;
using ProcessTracker.API.Services;

namespace ProcessTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ApplicationsController : ControllerBase
{
    private readonly IApplicationService _service;

    public ApplicationsController(IApplicationService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<ApplicationDto>))]
    public async Task<ActionResult<IEnumerable<ApplicationDto>>> GetAll([FromQuery] int? projectId)
    {
        var apps = await _service.GetAllAsync(projectId);
        return Ok(apps);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> GetById(int id)
    {
        var app = await _service.GetByIdAsync(id);
        if (app == null) return NotFound();
        return Ok(app);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApplicationDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ApplicationDto>> Create(ApplicationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var created = await _service.CreateAsync(dto);
        if (created == null) return BadRequest("Associated Project not found.");
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApplicationDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ApplicationDto>> Update(int id, ApplicationDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (id != dto.Id) return BadRequest();
        var updated = await _service.UpdateAsync(id, dto);
        if (updated == null) return NotFound();
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _service.DeleteAsync(id);
            if (!result) return NotFound();
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { message = ex.Message });
        }
    }
}
