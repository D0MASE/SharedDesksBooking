using SharedDesksBooking.Models;
using SharedDesksBooking.Validators;
using Xunit;

namespace SharedDesksBooking.Tests
{
    public class ValidatorTests
    {
        private readonly CreateReservationRequestValidator _validator = new();

        [Fact]
        public void CreateReservationRequest_Validation_Fails_When_Empty()
        {
            var request = new CreateReservationRequest();
            var result = _validator.Validate(request);
            Assert.False(result.IsValid);
        }

        [Fact]
        public void CreateReservationRequest_Validation_Fails_When_Name_Too_Short()
        {
            var request = new CreateReservationRequest 
            { 
                DeskId = 1, 
                FirstName = "A", 
                LastName = "Doe",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };
            var result = _validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "FirstName");
        }

        [Fact]
        public void CreateReservationRequest_Validation_Fails_When_Dates_Invalid()
        {
            var request = new CreateReservationRequest 
            { 
                DeskId = 1, 
                FirstName = "John", 
                LastName = "Doe",
                StartDate = DateTime.Today.AddDays(2),
                EndDate = DateTime.Today.AddDays(1)
            };
            var result = _validator.Validate(request);
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, e => e.PropertyName == "EndDate");
        }

        [Fact]
        public void CreateReservationRequest_Validation_Succeeds_When_Valid()
        {
            var request = new CreateReservationRequest 
            { 
                DeskId = 1, 
                FirstName = "John", 
                LastName = "Doe",
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddDays(1)
            };
            var result = _validator.Validate(request);
            Assert.True(result.IsValid);
        }
    }
}
