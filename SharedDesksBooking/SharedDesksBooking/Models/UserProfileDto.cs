namespace SharedDesksBooking.Models
{
    public class UserProfileDto
    {
        public required string FirstName { get; init; }
        public required string LastName { get; init; }

        public List<UserReservationDto> CurrentReservations { get; init; } = new();
        public List<UserReservationDto> PastReservations { get; init; } = new();
    }

    public class UserReservationDto
    {
        public int Id { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
        public int DeskId { get; init; }
        public required string DeskNumber { get; init; }
    }
}