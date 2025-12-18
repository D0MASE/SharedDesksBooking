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
        public async Task<IActionResult> GetDesks()
        {
            var desks = await _context.Desks.ToListAsync();

            var todayReservations = await _context.Reservations
                .Where(r => r.ReservedDate.Date == DateTime.Today.Date)
                .ToListAsync();

            var response = desks.Select(d => new
            {
                Id = d.Id,
                Number = d.Number,
                IsUnderMaintenance = d.IsUnderMaintenance,

                Reservation = todayReservations.FirstOrDefault(r => r.DeskId == d.Id)
            });

            return Ok(response);
        }
    }
}
