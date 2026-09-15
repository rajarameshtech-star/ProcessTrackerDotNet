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
    public async Task<IActionResult> GetById(int id)
    {
        var record = await _service.GetByIdAsync(id);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpGet("by-service-item/{serviceItemId}")]
    public async Task<IActionResult> GetByServiceItem(int serviceItemId)
    {
        var record = await _service.GetByServiceItemIdAsync(serviceItemId);
        if (record == null) return NotFound();
        return Ok(record);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProcessRecordDto dto)
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
    public async Task<IActionResult> Update(int id, ProcessRecordDto dto)
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
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.DeleteAsync(id);
        if (!result) return NotFound();
        return Ok();
    }
}
