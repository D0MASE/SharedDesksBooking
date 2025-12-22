using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Controllers;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;
using Xunit;

namespace SharedDesksBooking.Tests
{
    public class DesksControllerTests
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

        [Fact]
        public async Task GetDesks_ReturnsAllDesks_EvenIfNoReservationsExist()
        {
            // Scenario: Database has 2 desks, no reservations.
            var context = GetDatabaseContext();
            context.Desks.AddRange(
                new Desk { Id = 1, Number = "A1" },
                new Desk { Id = 2, Number = "A2" }
            );
            await context.SaveChangesAsync();

            var controller = new DesksController(context);

            var result = await controller.GetDesks(null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(2, response.Count());
        }

        [Fact]
        public async Task GetDesks_ReturnsReservationDetails_WhenDeskIsOccupiedOnTargetDate()
        {
            // Scenario: Desk is reserved exactly for the requested date.
            var context = GetDatabaseContext();
            var targetDate = new DateTime(2025, 10, 10);

            context.Desks.Add(new Desk { Id = 1, Number = "B1" });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "John",
                LastName = "Doe",
                StartDate = targetDate,
                EndDate = targetDate
            });
            await context.SaveChangesAsync();

            var controller = new DesksController(context);

            var result = await controller.GetDesks(targetDate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            var firstDesk = Enumerable.First(response);

            Assert.NotNull(firstDesk.Reservation);
            Assert.Equal("John", firstDesk.Reservation.FirstName);
        }

        [Fact]
        public async Task GetDesks_ReturnsReservation_WhenDateIsWithinRange()
        {
            // Scenario: Reservation is from 10th to 15th. We check 12th.
            var context = GetDatabaseContext();
            var startDate = new DateTime(2025, 10, 10);
            var checkDate = new DateTime(2025, 10, 12);
            var endDate = new DateTime(2025, 10, 15);

            context.Desks.Add(new Desk { Id = 1, Number = "C1" });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                StartDate = startDate,
                EndDate = endDate
            });
            await context.SaveChangesAsync();

            var controller = new DesksController(context);

            var result = await controller.GetDesks(checkDate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            var desk = Enumerable.First(response);

            Assert.NotNull(desk.Reservation);
            Assert.Equal("Alice", desk.Reservation.FirstName);
        }

        [Fact]
        public async Task GetDesks_ReturnsNullReservation_WhenDateIsOutsideRange()
        {
            // Scenario: Desk is reserved tomorrow, but we check today.
            var context = GetDatabaseContext();
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            context.Desks.Add(new Desk { Id = 1, Number = "D1" });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "FutureUser",
                LastName = "Test",
                StartDate = tomorrow,
                EndDate = tomorrow
            });
            await context.SaveChangesAsync();

            var controller = new DesksController(context);

            var result = await controller.GetDesks(today);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            var desk = Enumerable.First(response);

            // Reservation should be null because it hasn't started yet
            Assert.Null(desk.Reservation);
        }

        [Fact]
        public async Task GetDesks_CorrectlyShowsMaintenanceStatus()
        {
            // Scenario: One desk is under maintenance, another is not.
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "M1", IsUnderMaintenance = true });
            context.Desks.Add(new Desk { Id = 2, Number = "M2", IsUnderMaintenance = false });
            await context.SaveChangesAsync();

            var controller = new DesksController(context);

            var result = await controller.GetDesks(null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);

            var desk1 = Enumerable.ElementAt(response, 0);
            var desk2 = Enumerable.ElementAt(response, 1);

            Assert.True(desk1.IsUnderMaintenance);
            Assert.False(desk2.IsUnderMaintenance);
        }

        [Fact]
        public async Task GetDesks_DefaultsToToday_WhenNoDateProvided()
        {
            // Scenario: Call GetDesks() without parameters. Should use DateTime.Today.
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "T1" });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "TodayUser",
                LastName = "Test",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            });
            await context.SaveChangesAsync();

            var controller = new DesksController(context);

            var result = await controller.GetDesks(null); // passing null

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            var desk = Enumerable.First(response);

            Assert.NotNull(desk.Reservation);
            Assert.Equal("TodayUser", desk.Reservation.FirstName);
        }
    }
}