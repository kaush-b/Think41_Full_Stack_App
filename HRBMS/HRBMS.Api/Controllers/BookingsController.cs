using System.Security.Claims;
using HRBMS.Core.Entities;
using HRBMS.Core.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HRBMS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingsController(IBookingService bookingService)
    {
        _bookingService = bookingService;
    }

    public record CreateBookingRequest(int RoomId, DateTime CheckIn, DateTime CheckOut);

    [HttpGet]
    [Authorize(Policy = "RequireStaffOrAdmin")]
    public async Task<IEnumerable<Booking>> GetAll(CancellationToken cancellationToken)
    {
        return await _bookingService.GetAllBookingsAsync(cancellationToken);
    }

    [HttpGet("my")]
    [Authorize]
    public async Task<IEnumerable<Booking>> GetMine(CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")!.Value);
        return await _bookingService.GetBookingsForUserAsync(userId, cancellationToken);
    }

    [HttpPost]
    [Authorize(Policy = "RequireGuest")]
    public async Task<ActionResult<Booking>> Create([FromBody] CreateBookingRequest request, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")!.Value);
        var booking = await _bookingService.CreateBookingAsync(request.RoomId, userId, request.CheckIn, request.CheckOut, cancellationToken);
        return CreatedAtAction(nameof(GetMine), new { id = booking.Id }, booking);
    }

    [HttpPost("{id:int}/cancel")]
    [Authorize]
    public async Task<IActionResult> Cancel(int id, CancellationToken cancellationToken)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? User.FindFirst("sub")!.Value);
        var isStaffOrAdmin = User.IsInRole("Staff") || User.IsInRole("Admin");
        var ok = await _bookingService.CancelBookingAsync(id, userId, isStaffOrAdmin, cancellationToken);
        return ok ? NoContent() : NotFound();
    }

    [HttpPost("{id:int}/status")]
    [Authorize(Policy = "RequireStaffOrAdmin")]
    public async Task<ActionResult<Booking?>> UpdateStatus(int id, [FromQuery] BookingStatus status, CancellationToken cancellationToken)
    {
        var updated = await _bookingService.UpdateStatusAsync(id, status, cancellationToken);
        if (updated == null) return NotFound();
        return Ok(updated);
    }
}