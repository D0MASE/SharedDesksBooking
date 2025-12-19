using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        //POST: api/reservations
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            // Basic validation: check if the desk exists and is not under maintenance
            var desk = await _context.Desks.FindAsync(reservation.DeskId);
            if (desk == null || desk.IsUnderMaintenance)
            {
                return BadRequest("Desk is not available for reservation.");
            }
            // Logic to check if the desk is already reserved for the chosen period
            var isOccupied = await _context.Reservations.AnyAsync(r =>
                r.DeskId == reservation.DeskId &&
                reservation.StartDate.Date <= r.EndDate.Date &&
                reservation.EndDate.Date >= r.StartDate.Date
            );

            if (isOccupied) return BadRequest("Desk is already reserved for these dates.");

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return Ok(reservation);

        }

        //DELETE: api/reservations/{id}
        [HttpDelete("{id}")]
        // Handles cancelling for one day or the whole rang
        public IActionResult CacelReservation(int id, [FromQuery] bool onlyToday, [FromQuery] DateTime date)
        {
            var res = _context.Reservations.FirstOrDefault(r => r.Id == id);

            if (res == null) return NotFound("Reservation not found");

            var targetDate = date.Date;

            if (onlyToday)
            {
                // Logic to split or adjust the reservation for a specific day
                if (res.StartDate.Date == res.EndDate.Date)
                {
                    _context.Reservations.Remove(res);
                }
                else if (res.StartDate.Date == targetDate)
                {
                    res.StartDate = targetDate.AddDays(1);
                }
                else if (res.EndDate.Date == targetDate)
                {
                    res.EndDate = targetDate.AddDays(-1);
                }
                else
                {
                    // Split into two reservations
                    var secondPart = new Reservation
                    {
                        DeskId = res.DeskId,
                        FirstName = res.FirstName,
                        LastName = res.LastName,
                        StartDate = targetDate.AddDays(1),
                        EndDate = res.EndDate
                    };

                    res.EndDate = targetDate.AddDays(-1);
                    _context.Reservations.Add(secondPart);
                }
            }
            else
            {
                // Cancel whole range
                _context.Reservations.Remove(res);
            }

            _context.SaveChanges();
            return Ok();
        }
    }
}
