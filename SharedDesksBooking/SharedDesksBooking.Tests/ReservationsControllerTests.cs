using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Controllers;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;
using Xunit;

namespace SharedDesksBooking.Tests
{
    public class ReservationsControllerTests
    {
        private AppDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new AppDbContext(options);
            databaseContext.Database.EnsureCreated();
            return databaseContext;
        }

        #region POST: CreateReservation Tests

        [Fact]
        public async Task CreateReservation_ReturnsOk_WhenValid()
        {
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "A1", IsUnderMaintenance = false });
            await context.SaveChangesAsync();

            var controller = new ReservationsController(context);
            var res = new Reservation { 
                DeskId = 1, 
                StartDate = DateTime.Today.AddDays(1), 
                EndDate = DateTime.Today.AddDays(2),
                FirstName = "John", LastName = "Doe" 
            };

            var result = await controller.CreateReservation(res);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateReservation_ReturnsBadRequest_WhenInPast()
        {
            var context = GetDatabaseContext();
            var controller = new ReservationsController(context);
            var res = new Reservation { 
                FirstName = "John", LastName = "Doe",
                StartDate = DateTime.Today.AddDays(-1), 
                EndDate = DateTime.Today 
            };

            var result = await controller.CreateReservation(res);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cannot book in the past.", badRequest.Value);
        }

        [Fact]
        public async Task CreateReservation_ReturnsBadRequest_WhenDeskUnderMaintenance()
        {
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "M1", IsUnderMaintenance = true });
            await context.SaveChangesAsync();

            var controller = new ReservationsController(context);
            var res = new Reservation { 
                DeskId = 1, 
                FirstName = "John", LastName = "Doe",
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today 
            };

            var result = await controller.CreateReservation(res);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Desk is under maintenance.", badRequest.Value);
        }

        [Fact]
        public async Task CreateReservation_ReturnsBadRequest_WhenOverlapping()
        {
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "D1", IsUnderMaintenance = false });
            context.Reservations.Add(new Reservation { 
                FirstName = "Existing", LastName = "User",
                DeskId = 1, 
                StartDate = DateTime.Today.AddDays(1), 
                EndDate = DateTime.Today.AddDays(3) 
            });
            await context.SaveChangesAsync();

            var controller = new ReservationsController(context);
            var overlapRes = new Reservation { 
                DeskId = 1, 
                FirstName = "New", LastName = "User",
                StartDate = DateTime.Today.AddDays(2), 
                EndDate = DateTime.Today.AddDays(4) 
            };

            var result = await controller.CreateReservation(overlapRes);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Desk is already reserved for these dates.", badRequest.Value);
        }

        #endregion

        #region DELETE: CancelReservation Tests

        [Fact]
        public async Task CancelReservation_WholeRange_RemovesFromDb()
        {
            var context = GetDatabaseContext();
            context.Reservations.Add(new Reservation { 
                Id = 1, DeskId = 1, FirstName = "John", LastName = "Doe", 
                StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2) 
            });
            await context.SaveChangesAsync();

            var controller = new ReservationsController(context);
            await controller.CancelReservation(1, false, DateTime.Today);

            Assert.Empty(context.Reservations);
        }

        [Fact]
        public async Task CancelReservation_OnlyToday_SplitsIntoTwo()
        {
            var context = GetDatabaseContext();
            var res = new Reservation { 
                Id = 1, DeskId = 1, 
                StartDate = new DateTime(2025, 12, 10), 
                EndDate = new DateTime(2025, 12, 12),
                FirstName = "John", LastName = "Doe"
            };
            context.Reservations.Add(res);
            await context.SaveChangesAsync();

            var controller = new ReservationsController(context);
            await controller.CancelReservation(1, true, new DateTime(2025, 12, 11));

            var results = await context.Reservations.ToListAsync();
            Assert.Equal(2, results.Count);
            Assert.Contains(results, r => r.EndDate == new DateTime(2025, 12, 10));
            Assert.Contains(results, r => r.StartDate == new DateTime(2025, 12, 12));
        }

        [Fact]
        public async Task CancelReservation_OnlyToday_AdjustsStart()
        {
            var context = GetDatabaseContext();
            var res = new Reservation { 
                Id = 1, FirstName = "John", LastName = "Doe",
                StartDate = new DateTime(2025, 12, 10), 
                EndDate = new DateTime(2025, 12, 12) 
            };
            context.Reservations.Add(res);
            await context.SaveChangesAsync();

            var controller = new ReservationsController(context);
            await controller.CancelReservation(1, true, new DateTime(2025, 12, 10));

            var updatedRes = await context.Reservations.FirstAsync();
            Assert.Equal(new DateTime(2025, 12, 11), updatedRes.StartDate);
        }

        [Fact]
        public async Task CancelReservation_ReturnsBadRequest_IfDateOutsideRange()
        {
            var context = GetDatabaseContext();
            context.Reservations.Add(new Reservation { 
                Id = 1, FirstName = "John", LastName = "Doe",
                StartDate = DateTime.Today, EndDate = DateTime.Today 
            });
            await context.SaveChangesAsync();

            var controller = new ReservationsController(context);
            var result = await controller.CancelReservation(1, true, DateTime.Today.AddDays(1));

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Selected date is not within the reservation period.", badRequest.Value);
        }

        #endregion
    }
}