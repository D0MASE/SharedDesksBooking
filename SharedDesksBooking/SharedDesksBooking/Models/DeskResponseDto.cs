namespace SharedDesksBooking.Models
{
    /// <summary>
    /// Data Transfer Object for representing a desk with its current availability.
    /// </summary>
    public class DeskResponseDto
    {
        public int Id { get; set; }
        public string Number { get; set; } = string.Empty;
        public bool IsUnderMaintenance { get; set; }
        public ReservationDto? Reservation { get; set; }
    }

    /// <summary>
    /// Data Transfer Object for reservation details shown in the desk list.
    /// </summary>
    public class ReservationDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}