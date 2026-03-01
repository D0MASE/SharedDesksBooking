namespace SharedDesksBooking.Models;

public record UserProfileDto
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }

    public List<UserReservationDto> CurrentReservations { get; init; } = new();
    public List<UserReservationDto> PastReservations { get; init; } = new();
}