using SharedDesksBooking.Models;

namespace SharedDesksBooking.Services
{
    public interface IReservationService
    {
        Task<(bool Success, string Message)> CreateReservationAsync(CreateReservationRequest request);
        Task<(bool Success, string Message)> CancelReservationAsync(int id, bool onlyToday, DateTime date);
    }
}
