using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Services;

public class DeskService(AppDbContext context, IMapper mapper) : IDeskService
{
    public async Task<IEnumerable<DeskResponseDto>> GetDesksWithAvailabilityAsync(DateTime date)
    {
        var targetDate = date.Date;
        var desks = await context.Desks
            .Include(d => d.Reservations)
            .ToListAsync();

        return desks.Select(d =>
        {
            var dto = mapper.Map<DeskResponseDto>(d);
            var activeRes = d.Reservations.FirstOrDefault(r => targetDate >= r.StartDate.Date && targetDate <= r.EndDate.Date);
            if (activeRes != null)
            {
                return dto with { Reservation = mapper.Map<ReservationDto>(activeRes) };
            }
            return dto;
        });
    }
}