using SharedDesksBooking.Models;

namespace SharedDesksBooking.Data
{
    public static class SeedData
    {
        public static void Initialize(AppDbContext context)
        {
            if (context.Desks.Any()) return;

            context.Desks.AddRange(
                new Desk { Id = 1, Number = "A-1", IsUnderMaintenance = false },
                new Desk { Id = 2, Number = "A-2", IsUnderMaintenance = false },
                new Desk { Id = 3, Number = "B-1", IsUnderMaintenance = true },
                new Desk { Id = 4, Number = "B-2", IsUnderMaintenance = false }
            );

            context.Reservations.Add(new Reservation
            {
                Id = 1,
                DeskId = 1,
                FirstName = "Dominykas",
                LastName = "Asevicius",
                StartDate = DateTime.Today.AddDays(-1),
                EndDate = DateTime.Today.AddDays(2)
            });

            context.SaveChanges();
        }
    }
}
