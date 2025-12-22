using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;

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
            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
                return BadRequest("First and last names are required.");

            //Fetch all reservations for the specific user
            var userReservation = await _context.Reservations
                .Where(r => r.FirstName.ToLower() == firstName.ToLower() &&
                     r.LastName.ToLower() == lastName.ToLower())
                .Join(_context.Desks,
                    res => res.DeskId,
                    desk => desk.Id,
                    (res, desk) => new UserReservationDto // Mapping directly to DTO
                    {
                         Id = res.Id,
                         StartDate = res.StartDate,
                         EndDate = res.EndDate,
                         DeskId = res.DeskId,
                         Number = desk.Number
                     })
         .OrderByDescending(r => r.StartDate)
         .ToListAsync();

            var today = DateTime.Today;

            // Seperate reservations into current/future and past
            var response = new UserProfileDto
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

