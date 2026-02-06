using SharedDesksBooking.Models;
using SharedDesksBooking.Models.Enums;

namespace SharedDesksBooking.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        // If database is already seeded, do nothing
        if (context.Desks.Any() || context.Reservations.Any()) return;

        // 1. Seed Desks
        var desks = new List<Desk>
        {
            new Desk { Id = 1, Number = "A-1", Status = DeskStatus.Available },
            new Desk { Id = 2, Number = "A-2", Status = DeskStatus.Available },
            new Desk { Id = 3, Number = "B-1", Status = DeskStatus.UnderMaintenance },
            new Desk { Id = 4, Number = "B-2", Status = DeskStatus.Available },
            new Desk { Id = 5, Number = "C-1", Status = DeskStatus.Available }
        };

        context.Desks.AddRange(desks);

        // 2. Seed Reservations
        var today = DateTime.Today;

        context.Reservations.AddRange(
            // Current/Active reservation for you
            new Reservation
            {
                Id = 1,
                DeskId = 1,
                FirstName = "Dominykas",
                LastName = "Asevicius",
                StartDate = today.AddDays(-1),
                EndDate = today.AddDays(2)
            },
            // Past reservation
            new Reservation
            {
                Id = 2,
                DeskId = 2,
                FirstName = "Dominykas",
                LastName = "Asevicius",
                StartDate = today.AddDays(-10),
                EndDate = today.AddDays(-8)
            },
            // Future reservation
            new Reservation
            {
                Id = 3,
                DeskId = 4,
                FirstName = "Jonas",
                LastName = "Jonaitis",
                StartDate = today.AddDays(5),
                EndDate = today.AddDays(7)
            },
            // Current reservation
            new Reservation
            {
                Id = 4,
                DeskId = 5,
                FirstName = "Petras",
                LastName = "Petraitis",
                StartDate = today,
                EndDate = today
            }
        );

        context.SaveChanges();
    }
}