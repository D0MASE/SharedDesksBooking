namespace SharedDesksBooking.Models;

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
