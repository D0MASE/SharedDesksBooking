using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DesksController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DesksController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Fetches all desks and their availability status for a specific date.
        /// </summary>
        /// <param name="date">The date to check availability for. Defaults to today.</param>
        /// <returns>A list of desks with their maintenance status and active reservation details.</returns>
        [HttpGet]
        public async Task<IActionResult> GetDesks([FromQuery] DateTime? date)
        {

            var targetDate = (date ?? DateTime.Today).Date;

            var desks = await _context.Desks.ToListAsync();

            var activeReservations = await _context.Reservations
                .Where(r => targetDate >= r.StartDate.Date && targetDate <= r.EndDate.Date)
                .ToListAsync();

            var response = desks.Select(d => new DeskResponseDto
            {
                Id = d.Id,
                Number = d.Number,
                IsUnderMaintenance = d.IsUnderMaintenance,

                Reservation = activeReservations
                .Where(r => r.DeskId == d.Id)
                .Select(r => new ReservationDto
                {
                    Id = r.Id,
                    FirstName = r.FirstName,
                    LastName = r.LastName,
                    StartDate = r.StartDate,
                    EndDate = r.EndDate
                })
                .FirstOrDefault()
            });

            return Ok(response);
        }
    }
}
