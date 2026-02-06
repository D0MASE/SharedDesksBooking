using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;
using SharedDesksBooking.Models.Enums;

namespace SharedDesksBooking.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        private readonly IMapper _mapper;
        public ReservationService(AppDbContext context, IMapper mapper) 
        { 
            _mapper = mapper;
            _context = context; 
        }

        public async Task<(bool Success, string Message)> CreateReservationAsync(CreateReservationRequest request)
        {
            var desk = await _context.Desks.FindAsync(request.DeskId);

            if (desk == null)
                return (false, "Stalas neegzistuoja.");

            // 2. Patikriname, ar stalas nėra remontuojamas
            if (desk.Status != DeskStatus.Available)
            {
                string reason = desk.Status == DeskStatus.UnderMaintenance
                    ? "remontuojamas"
                    : "nepasiekiamas";

                return (false, "Šis stalas šiuo metu remontuojamas, rezervacija negalima.");
            }
               
            // Logic to check if the desk is already reserved for the chosen period
            var isOccupied = await _context.Reservations.AnyAsync(r =>
                r.DeskId == request.DeskId &&
                request.StartDate.Date <= r.EndDate.Date &&
                request.EndDate.Date >= r.StartDate.Date
            );

            if (isOccupied) return (false, "Desk is already reserved for these dates.");

            // MAPPING: Iš DTO sukuriame tikrą Reservation objektą
            var reservation = _mapper.Map<Reservation>(request);

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }

        public async Task<(bool Success, string Message)> CancelReservationAsync(int id, bool onlyToday, DateTime date)
        {
            var res = await _context.Reservations.FirstOrDefaultAsync(r => r.Id == id);
            if (res == null) return (false, "Reservation not found.");

            var targetDate = date.Date;

            if (onlyToday)
            {
                if (targetDate < res.StartDate.Date || targetDate > res.EndDate.Date)
                    return (false, "Selected date is not within the request period.");

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
                    var secondPart = _mapper.Map<Reservation>(res);
                    res.EndDate = targetDate.AddDays(-1);
                    _context.Reservations.Add(secondPart);
                }
            }
            else
            {
                _context.Reservations.Remove(res);
            }

            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }
    }
}
