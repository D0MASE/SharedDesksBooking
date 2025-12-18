using Microsoft.AspNetCore.Http;
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

        [HttpGet]
        public async Task<IActionResult> GetDesks([FromQuery] DateTime? date)
        {
            var desks = await _context.Desks.ToListAsync();

            var targetDate = date ?? DateTime.Today;

            var activeReservations = await _context.Reservations
                .Where(r => targetDate.Date >= r.StartDate.Date && targetDate.Date <= r.EndDate.Date)
                .ToListAsync();

            var response = desks.Select(d => new
            {
                Id = d.Id,
                Number = d.Number,
                IsUnderMaintenance = d.IsUnderMaintenance,

                Reservation = activeReservations.FirstOrDefault(r => r.DeskId == d.Id)
            });

            return Ok(response);
        }

        [HttpDelete("reservation/{id}")]
        public IActionResult CacelReservation(int id, [FromQuery] bool onlyToday)
        {
            var res = _context.Reservations.FirstOrDefault(r => r.Id == id);

            if (res == null) return NotFound("Reservation not found");

            var today = DateTime.Today;

            if (onlyToday)
            {
                if (res.StartDate.Date == res.EndDate.Date)
                {
                    _context.Reservations.Remove(res);
                }
                else if (res.StartDate.Date == today)
                {
                    res.StartDate = today.AddDays(1);
                }
                else if (res.EndDate.Date == today)
                {
                    res.EndDate = today.AddDays(-1);
                }
                else
                {
                    var secondPart = new Reservation
                    {
                        DeskId = res.DeskId,
                        FirstName = res.FirstName,
                        LastName = res.LastName,
                        StartDate = today.AddDays(1),
                        EndDate = res.EndDate
                    };

                    res.EndDate = today.AddDays(-1);
                    _context.Reservations.Add(secondPart);
                }
            }
            else
            {
                _context.Reservations.Remove(res);
            }

            _context.SaveChanges();
            return Ok();
        }
    }
}
