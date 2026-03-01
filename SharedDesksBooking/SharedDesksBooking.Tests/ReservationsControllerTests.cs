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
    public class ReservationsControllerTests
    {
        private IMapper _mapper;
        
        public ReservationsControllerTests()
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
        public async Task CreateReservation_ReturnsOk_WhenValid()
        {
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "A1", Status = DeskStatus.Available });
            await context.SaveChangesAsync();

            var service = new ReservationService(context, _mapper);
            var controller = new ReservationsController(service);
            
            var request = new CreateReservationRequest { 
                DeskId = 1, 
                StartDate = DateTime.Today.AddDays(1), 
                EndDate = DateTime.Today.AddDays(2),
                FirstName = "John", 
                LastName = "Doe" 
            };

            var result = await controller.CreateReservation(request);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task CreateReservation_ReturnsBadRequest_WhenInPast()
        {
            var context = GetDatabaseContext();
            var service = new ReservationService(context, _mapper);
            var controller = new ReservationsController(service);
            var request = new CreateReservationRequest { 
                FirstName = "John", LastName = "Doe",
                StartDate = DateTime.Today.AddDays(-1), 
                EndDate = DateTime.Today 
            };

            var result = await controller.CreateReservation(request);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cannot book in the past.", badRequest.Value);
        }

        [Fact]
        public async Task CreateReservation_ReturnsBadRequest_WhenDeskUnderMaintenance()
        {
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "M1", Status = DeskStatus.UnderMaintenance });
            await context.SaveChangesAsync();

            var service = new ReservationService(context, _mapper);
            var controller = new ReservationsController(service);
            var request = new CreateReservationRequest { 
                DeskId = 1, 
                FirstName = "John", LastName = "Doe",
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today 
            };

            var result = await controller.CreateReservation(request);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Contains("remontuojamas", badRequest.Value.ToString());
        }

        [Fact]
        public async Task CreateReservation_ReturnsBadRequest_WhenOverlapping()
        {
            var context = GetDatabaseContext();
            context.Desks.Add(new Desk { Id = 1, Number = "D1", Status = DeskStatus.Available });
            context.Reservations.Add(new Reservation { 
                FirstName = "Existing", LastName = "User",
                DeskId = 1, 
                StartDate = DateTime.Today.AddDays(1), 
                EndDate = DateTime.Today.AddDays(3) 
            });
            await context.SaveChangesAsync();

            var service = new ReservationService(context, _mapper);
            var controller = new ReservationsController(service);
            var overlapRequest = new CreateReservationRequest { 
                DeskId = 1, 
                FirstName = "New", LastName = "User",
                StartDate = DateTime.Today.AddDays(2), 
                EndDate = DateTime.Today.AddDays(4) 
            };

            var result = await controller.CreateReservation(overlapRequest);
            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Desk is already reserved for these dates.", badRequest.Value);
        }

        [Fact]
        public async Task CancelReservation_WholeRange_RemovesFromDb()
        {
            var context = GetDatabaseContext();
            context.Reservations.Add(new Reservation { 
                Id = 1, DeskId = 1, FirstName = "John", LastName = "Doe", 
                StartDate = DateTime.Today, EndDate = DateTime.Today.AddDays(2) 
            });
            await context.SaveChangesAsync();

            var service = new ReservationService(context, _mapper);
            var controller = new ReservationsController(service);
            
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

            var service = new ReservationService(context, _mapper);
            var controller = new ReservationsController(service);
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
                Id = 1, DeskId = 1, FirstName = "John", LastName = "Doe",
                StartDate = new DateTime(2025, 12, 10), 
                EndDate = new DateTime(2025, 12, 12) 
            };
            context.Reservations.Add(res);
            await context.SaveChangesAsync();

            var service = new ReservationService(context, _mapper);
            var controller = new ReservationsController(service);
            await controller.CancelReservation(1, true, new DateTime(2025, 12, 10));

            var updatedRes = await context.Reservations.FirstAsync();
            Assert.Equal(new DateTime(2025, 12, 11), updatedRes.StartDate);
        }

        [Fact]
        public async Task CancelReservation_ReturnsBadRequest_IfDateOutsideRange()
        {
            var context = GetDatabaseContext();
            context.Reservations.Add(new Reservation { 
                Id = 1, DeskId = 1, FirstName = "John", LastName = "Doe",
                StartDate = DateTime.Today, EndDate = DateTime.Today 
            });
            await context.SaveChangesAsync();

            var service = new ReservationService(context, _mapper);
            var controller = new ReservationsController(service);
            var result = await controller.CancelReservation(1, true, DateTime.Today.AddDays(1));

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Selected date is not within the request period.", badRequest.Value);
        }
    }
}