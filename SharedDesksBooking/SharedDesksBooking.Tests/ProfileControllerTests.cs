using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Controllers;
using SharedDesksBooking.Data;
using SharedDesksBooking.Models;
using Xunit;

namespace SharedDesksBooking.Tests
{
    public class ProfileControllerTests
    {
        private AppDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetUserProfile_ReturnsBadRequest_WhenNamesAreEmpty()
        {
            var context = GetDatabaseContext();
            var controller = new ProfileController(context);

            var result = await controller.GetUserProfile("", "");

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task GetUserProfile_CorrectlySplitsPastAndFuture()
        {
            // Arrange
            var context = GetDatabaseContext();
            var today = DateTime.Today;
            context.Desks.Add(new Desk { Id = 1, Number = "A1" });

            // Past reservation (ended yesterday)
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "John",
                LastName = "Doe",
                StartDate = today.AddDays(-5),
                EndDate = today.AddDays(-1)
            });

            // Current reservation (ends today)
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "John",
                LastName = "Doe",
                StartDate = today,
                EndDate = today
            });

            await context.SaveChangesAsync();
            var controller = new ProfileController(context);

            // Act
            var result = await controller.GetUserProfile("John", "Doe");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<UserProfileDto>(okResult.Value);

            Assert.Single(profile.PastReservations);
            Assert.Single(profile.CurrentReservations);
            Assert.Equal("A1", profile.CurrentReservations[0].Number);
        }

        [Fact]
        public async Task GetUserProfile_IsCaseInsensitive()
        {
            // Arrange
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "A1" });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "John",
                LastName = "Doe",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            });
            await context.SaveChangesAsync();
            var controller = new ProfileController(context);

            // Act: Search with lowercase
            var result = await controller.GetUserProfile("john", "doe");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<UserProfileDto>(okResult.Value);
            Assert.NotEmpty(profile.CurrentReservations);
        }

        [Fact]
        public async Task GetUserProfile_ReturnsEmptyLists_ForUnknownUser()
        {
            var context = GetDatabaseContext();
            var controller = new ProfileController(context);

            var result = await controller.GetUserProfile("Ghost", "User");

            var okResult = Assert.IsType<OkObjectResult>(result);
            var profile = Assert.IsType<UserProfileDto>(okResult.Value);
            Assert.Empty(profile.CurrentReservations);
            Assert.Empty(profile.PastReservations);
        }
    }
}