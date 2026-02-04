using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Services
{
    public class DeskService : IDeskService
    {
        private readonly AppDbContext _context;

        public DeskService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DeskResponseDto>> GetDesksWithAvailabilityAsync(DateTime date)
        {
            var targetDate = date.Date;
            var desks = await _context.Desks.ToListAsync();
            var activeReservations = await _context.Reservations
                .Where(r => targetDate >= r.StartDate.Date && targetDate <= r.EndDate.Date)
                .ToListAsync();

            return desks.Select(d => new DeskResponseDto
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
                    }).FirstOrDefault()
            });
        }
    }
}