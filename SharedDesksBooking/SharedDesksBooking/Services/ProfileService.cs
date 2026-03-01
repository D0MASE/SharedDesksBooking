using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;

namespace SharedDesksBooking.Services;

public class ProfileService(AppDbContext context) : IProfileService
{
    public async Task<UserProfileDto?> GetUserProfileAsync(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName))
            return null;

        // Gauname visas rezervacijas ir sujungiam su stalo informacija
        var userReservations = await context.Reservations
            .Where(r => r.FirstName.ToLower() == firstName.ToLower() &&
                        r.LastName.ToLower() == lastName.ToLower())
            .Join(context.Desks,
                res => res.DeskId,
                desk => desk.Id,
                (res, desk) => new UserReservationDto
                {
                    Id = res.Id,
                    StartDate = res.StartDate,
                    EndDate = res.EndDate,
                    DeskId = res.DeskId,
                    DeskNumber = desk.Number
                })
            .OrderByDescending(r => r.StartDate)
            .ToListAsync();

        var today = DateTime.Today;

        return new UserProfileDto
        {
            FirstName = firstName,
            LastName = lastName,
            CurrentReservations = userReservations.Where(r => r.EndDate.Date >= today).ToList(),
            PastReservations = userReservations.Where(r => r.EndDate.Date < today).ToList(),
        };
    }
}