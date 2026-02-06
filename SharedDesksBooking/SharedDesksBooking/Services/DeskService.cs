using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Services
{
    public class DeskService : IDeskService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public DeskService(AppDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DeskResponseDto>> GetDesksWithAvailabilityAsync(DateTime date)
        {
            var targetDate = date.Date;
            var desks = await _context.Desks.ToListAsync();
            var activeReservations = await _context.Reservations
                .Where(r => targetDate >= r.StartDate.Date && targetDate <= r.EndDate.Date)
                .ToListAsync();

            return desks.Select(d =>
            {
                var dto = _mapper.Map<DeskResponseDto>(d);
                var activeRes = activeReservations.FirstOrDefault(r => r.DeskId == d.Id);
                if (activeRes != null)
                {
                    return dto with { Reservation = _mapper.Map<ReservationDto>(activeRes) };
                }
                return dto;
            });
        }
    }
}