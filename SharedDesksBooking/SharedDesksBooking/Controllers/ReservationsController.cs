using Microsoft.AspNetCore.Mvc;
using SharedDesksBooking.Models;
using SharedDesksBooking.Services;

namespace SharedDesksBooking.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ReservationsController(IReservationService reservationService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateReservation([FromBody] CreateReservationRequest request)
    {
        var (success, message) = await reservationService.CreateReservationAsync(request);

        if (!success) return BadRequest(message);

        return Ok(request);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> CancelReservation(int id, [FromQuery] bool onlyToday, [FromQuery] DateTime date)
    {
        var (success, message) = await reservationService.CancelReservationAsync(id, onlyToday, date);

        if (!success) return BadRequest(message);

        return Ok();
    }
}