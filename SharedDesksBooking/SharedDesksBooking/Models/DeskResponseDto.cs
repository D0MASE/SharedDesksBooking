namespace SharedDesksBooking.Models;

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