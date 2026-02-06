namespace SharedDesksBooking.Models
{
    /// <summary>
    /// Data Transfer Object for representing a desk with its current availability.
    /// </summary>
    public record DeskResponseDto
    {
        public int Id { get; init; } // 'init' reiškia, kad ID negalima keisti po sukūrimo
        public required string Number { get; init; }
        public required string Status { get; init; }
        public ReservationDto? Reservation { get; init; }
    }

    /// <summary>
    /// Data Transfer Object for reservation details shown in the desk list.
    /// </summary>
    public record ReservationDto
    {
        public int Id { get; init; }
        public required string FirstName { get; init; }
        public required string LastName { get; init; }
        public DateTime StartDate { get; init; }
        public DateTime EndDate { get; init; }
    }
}