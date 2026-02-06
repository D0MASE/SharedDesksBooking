using SharedDesksBooking.Models;

namespace SharedDesksBooking.Services;

public interface IProfileService
{
    Task<UserProfileDto?> GetUserProfileAsync(string firstName, string lastName);
}
