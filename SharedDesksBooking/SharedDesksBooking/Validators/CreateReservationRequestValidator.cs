using FluentValidation;
using SharedDesksBooking.Models; // Įsitikink, kad namespace sutampa

namespace SharedDesksBooking.Validators
{
    public class CreateReservationRequestValidator : AbstractValidator<CreateReservationRequest>
    {
        public CreateReservationRequestValidator()
        {
            RuleFor(x => x.DeskId)
                .NotEmpty().WithMessage("Stalas privalo būti pasirinktas.");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Vardas yra privalomas.")
                .Length(2, 50).WithMessage("Vardas turi būti nuo 2 iki 50 simbolių.");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Pavardė yra privaloma.");

            RuleFor(x => x.StartDate.Date)
                .GreaterThanOrEqualTo(DateTime.Today).WithMessage("Rezervacija negali būti praeityje.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("Pabaigos data turi būti vėlesnė už pradžios datą.");
        }
    }
}