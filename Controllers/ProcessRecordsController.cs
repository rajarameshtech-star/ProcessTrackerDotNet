using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessTracker.API.DTOs;
using ProcessTracker.API.Services;

namespace ProcessTracker.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProcessRecordsController : ControllerBase
{
    private readonly IProcessRecordService _service;

    public ProcessRecordsController(IProcessRecordService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessRecordDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessRecordDto>> GetById(int id)
    {
        var record = await _service.GetByIdAsync(id);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpGet("by-service-item/{serviceItemId}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessRecordDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessRecordDto>> GetByServiceItem(int serviceItemId)
    {
        var record = await _service.GetByServiceItemIdAsync(serviceItemId);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProcessRecordDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<ProcessRecordDto>> Create(ProcessRecordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        var (record, validationResult) = await _service.CreateAsync(dto);
        
        if (!validationResult.IsValid)
        {
            if (validationResult.Errors.Any(e => e.Message.Contains("ProcessRecord already exists")))
                return Conflict(validationResult.Errors);

            return BadRequest(validationResult.Errors);
        }

        return CreatedAtAction(nameof(GetById), new { id = record!.Id }, record);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProcessRecordDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProcessRecordDto>> Update(int id, ProcessRecordDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);
        if (id != dto.Id) return BadRequest();

        var (updated, validationResult) = await _service.UpdateAsync(id, dto);
        
        if (updated == null && validationResult.IsValid) 
            return NotFound();
            
        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        return Ok(updated);
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
