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

        /// <summary>
        /// Creates a new reservation for a specific desk and time period.
        /// </summary>
        /// <param name="reservation">The reservation details.</param>
        /// <returns>The created reservation object or an error message.</returns>
        [HttpPost]
        public async Task<IActionResult> CreateReservation([FromBody] Reservation reservation)
        {
            // Basic validation
            if (reservation.StartDate.Date < DateTime.Today)
                return BadRequest("Cannot book in the past.");

            if (reservation.EndDate < reservation.StartDate)
                return BadRequest("End date must be after start date.");
            var desk = await _context.Desks.FindAsync(reservation.DeskId);
            if (desk == null) return NotFound("Desk not found.");

            if (desk.IsUnderMaintenance)
                return BadRequest("Desk is under maintenance.");

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
        /// <summary>
        /// Cancels either a single day or the entire range of a reservation.
        /// </summary>
        /// <param name="id">The unique ID of the reservation.</param>
        /// <param name="onlyToday">If true, only the specified date is removed (splitting the reservation if needed).</param>
        /// <param name="date">The specific date to cancel when onlyToday is true.</param>
        /// <returns>Success or error status.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelReservation(int id, [FromQuery] bool onlyToday, [FromQuery] DateTime date)
        {
            var res = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (res == null) return NotFound("Reservation not found");

            var targetDate = date.Date;

            if (onlyToday)
            {
                if (targetDate < res.StartDate.Date || targetDate > res.EndDate.Date)
                    return BadRequest("Selected date is not within the reservation period.");
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
