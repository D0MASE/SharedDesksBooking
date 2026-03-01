namespace SharedDesksBooking.Models;

public record UserReservationDto
{
    public int Id { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public int DeskId { get; init; }
    public required string DeskNumber { get; init; }
}
