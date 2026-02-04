using SharedDesksBooking.Models;

namespace SharedDesksBooking.Services
{
    public interface IDeskService
    {
        Task<IEnumerable<DeskResponseDto>> GetDesksWithAvailabilityAsync(DateTime date);
    }
}
