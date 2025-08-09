using HRBMS.Core.Entities;
using HRBMS.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRBMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IEnumerable<Room>> GetAll(CancellationToken cancellationToken)
    {
        return await _roomService.GetAllAsync(cancellationToken);
    }

    [HttpGet("available")]
    [AllowAnonymous]
    public async Task<IEnumerable<Room>> GetAvailable([FromQuery] DateTime checkIn, [FromQuery] DateTime checkOut, CancellationToken cancellationToken)
    {
        return await _roomService.GetAvailableAsync(checkIn, checkOut, cancellationToken);
    }

    [HttpPost]
    [Authorize(Policy = "RequireStaffOrAdmin")]
    public async Task<ActionResult<Room>> Create([FromBody] Room room, CancellationToken cancellationToken)
    {
        var created = await _roomService.CreateRoomAsync(room, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [Authorize(Policy = "RequireStaffOrAdmin")]
    public async Task<ActionResult<Room?>> Update(int id, [FromBody] Room updated, CancellationToken cancellationToken)
    {
        var result = await _roomService.UpdateRoomAsync(id, updated, cancellationToken);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpDelete("{id:int}")]
    [Authorize(Policy = "RequireStaffOrAdmin")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var ok = await _roomService.DeleteRoomAsync(id, cancellationToken);
        return ok ? NoContent() : NotFound();
    }
}