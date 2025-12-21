using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;

namespace SharedDesksBooking.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves the reservation history for a specific user,
        /// seperated into current/future and past reservations.
        /// </summary>
        /// <param name="firstName">The first name of the user</param>
        /// <param name="lastName">The last name of the user</param>
        /// <returns>An object contraing user full name and lists of active and past reservations</returns>
        [HttpGet]
        public async Task<IActionResult> GetUserProfile([FromQuery] string firstName, [FromQuery] string lastName)
        {
            // Validation
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName)) return BadRequest("First and last names are required");

            //Fetch all reservations for the specific user
            var userReservation = await (from res in _context.Reservations
                                 join desk in _context.Desks on res.DeskId equals desk.Id
                                 where res.FirstName.ToLower() == firstName.ToLower() &&
                                       res.LastName.ToLower() == lastName.ToLower()
                                 select new
                                 {
                                     res.Id,
                                     res.StartDate,
                                     res.EndDate,
                                     res.DeskId,
                                     Number = desk.Number
                                 })
                         .OrderByDescending(r => r.StartDate)
                         .ToListAsync();

            var today = DateTime.Today;

            // Seperate reservations into current/future and past
            var response = new
            {
                FirstName = firstName,
                LastName = lastName,
                CurrentReservations = userReservation.Where(r => r.EndDate.Date >= today).ToList(),
                PastReservations = userReservation.Where(r => r.EndDate.Date < today).ToList(),
            };

            return Ok(response);
        }
    }
}

