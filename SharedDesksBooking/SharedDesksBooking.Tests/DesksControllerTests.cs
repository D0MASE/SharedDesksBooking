using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SharedDesksBooking.Controllers;
using SharedDesksBooking.Data;
using SharedDesksBooking.Mappings;
using SharedDesksBooking.Models;
using SharedDesksBooking.Models.Enums;
using SharedDesksBooking.Services;
using Xunit;

namespace SharedDesksBooking.Tests
{
    public class DesksControllerTests
    {
        private IMapper _mapper;
        
        public DesksControllerTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = config.CreateMapper();
        }

        private AppDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task GetDesks_ReturnsAllDesks_EvenIfNoReservationsExist()
        {
            var context = GetDatabaseContext();
            context.Desks.AddRange(
                new Desk { Id = 1, Number = "A1", Status = DeskStatus.Available },
                new Desk { Id = 2, Number = "A2", Status = DeskStatus.Available }
            );
            await context.SaveChangesAsync();

            var service = new DeskService(context, _mapper);
            var controller = new DesksController(service);

            var result = await controller.GetDesks(null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            Assert.Equal(2, response.Count());
        }

        [Fact]
        public async Task GetDesks_ReturnsReservationDetails_WhenDeskIsOccupiedOnTargetDate()
        {
            var context = GetDatabaseContext();
            var targetDate = new DateTime(2025, 10, 10);

            context.Desks.Add(new Desk { Id = 1, Number = "B1", Status = DeskStatus.Available });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "John",
                LastName = "Doe",
                StartDate = targetDate,
                EndDate = targetDate
            });
            await context.SaveChangesAsync();

            var service = new DeskService(context, _mapper);
            var controller = new DesksController(service);

            var result = await controller.GetDesks(targetDate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            var firstDesk = response.First();

            Assert.NotNull(firstDesk.Reservation);
            Assert.Equal("John", firstDesk.Reservation.FirstName);
        }

        [Fact]
        public async Task GetDesks_ReturnsReservation_WhenDateIsWithinRange()
        {
            var context = GetDatabaseContext();
            var startDate = new DateTime(2025, 10, 10);
            var checkDate = new DateTime(2025, 10, 12);
            var endDate = new DateTime(2025, 10, 15);

            context.Desks.Add(new Desk { Id = 1, Number = "C1", Status = DeskStatus.Available });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "Alice",
                LastName = "Smith",
                StartDate = startDate,
                EndDate = endDate
            });
            await context.SaveChangesAsync();

            var service = new DeskService(context, _mapper);
            var controller = new DesksController(service);

            var result = await controller.GetDesks(checkDate);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            var desk = response.First();

            Assert.NotNull(desk.Reservation);
            Assert.Equal("Alice", desk.Reservation.FirstName);
        }

        [Fact]
        public async Task GetDesks_ReturnsNullReservation_WhenDateIsOutsideRange()
        {
            var context = GetDatabaseContext();
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            context.Desks.Add(new Desk { Id = 1, Number = "D1", Status = DeskStatus.Available });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "FutureUser",
                LastName = "Test",
                StartDate = tomorrow,
                EndDate = tomorrow
            });
            await context.SaveChangesAsync();

            var service = new DeskService(context, _mapper);
            var controller = new DesksController(service);

            var result = await controller.GetDesks(today);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            var desk = response.First();

            Assert.Null(desk.Reservation);
        }

        [Fact]
        public async Task GetDesks_CorrectlyShowsMaintenanceStatus()
        {
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "M1", Status = DeskStatus.UnderMaintenance });
            context.Desks.Add(new Desk { Id = 2, Number = "M2", Status = DeskStatus.Available });
            await context.SaveChangesAsync();

            var service = new DeskService(context, _mapper);
            var controller = new DesksController(service);

            var result = await controller.GetDesks(null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);

            var desk1 = response.First(d => d.Number == "M1");
            var desk2 = response.First(d => d.Number == "M2");

            Assert.Equal("UnderMaintenance", desk1.Status);
            Assert.Equal("Available", desk2.Status);
        }

        [Fact]
        public async Task GetDesks_DefaultsToToday_WhenNoDateProvided()
        {
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "T1", Status = DeskStatus.Available });
            context.Reservations.Add(new Reservation
            {
                DeskId = 1,
                FirstName = "TodayUser",
                LastName = "Test",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            });
            await context.SaveChangesAsync();

            var service = new DeskService(context, _mapper);
            var controller = new DesksController(service);

            var result = await controller.GetDesks(null);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<IEnumerable<DeskResponseDto>>(okResult.Value);
            var desk = response.First();

            Assert.NotNull(desk.Reservation);
            Assert.Equal("TodayUser", desk.Reservation.FirstName);
        }
    }
}