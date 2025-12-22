namespace SharedDesksBooking.Models
{
    public class UserProfileDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public List<UserReservationDto> CurrentReservations { get; set; } = new();
        public List<UserReservationDto> PastReservations { get; set; } = new();
    }

    public class UserReservationDto
    {
        public int Id { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DeskId { get; set; }
        public string Number { get; set; } = string.Empty;
    }
}