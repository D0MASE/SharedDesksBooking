using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Services
{
    public class ReservationService : IReservationService
    {
        private readonly AppDbContext _context;
        public ReservationService(AppDbContext context) { _context = context; }

        public async Task<(bool Success, string Message)> CreateReservationAsync(Reservation reservation)
        {
            // Basic validation
            if (reservation.StartDate.Date < DateTime.Today)
                return (false, "Cannot book in the past.");

            if (reservation.EndDate < reservation.StartDate)
                return (false, "End date must be after start date.");
            var desk = await _context.Desks.FindAsync(reservation.DeskId);
            if (desk == null) return (false, "Desk not found.");

            if (desk.IsUnderMaintenance)
                return (false, "Desk is under maintenance.");

            // Logic to check if the desk is already reserved for the chosen period
            var isOccupied = await _context.Reservations.AnyAsync(r =>
                r.DeskId == reservation.DeskId &&
                reservation.StartDate.Date <= r.EndDate.Date &&
                reservation.EndDate.Date >= r.StartDate.Date
            );

            if (isOccupied) return (false, "Desk is already reserved for these dates.");

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
                    return (false, "Selected date is not within the reservation period.");

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
                _context.Reservations.Remove(res);
            }

            await _context.SaveChangesAsync();
            return (true, string.Empty);
        }
    }
}
