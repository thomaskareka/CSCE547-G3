using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface IBookingService {
        Task<Booking> GetBooking(Guid parkId);
        Task<List<Booking>> GetBookings();
        Task<bool> RemoveBooking(Guid bookingId);
        Task<bool> CreateBooking(Guid parkId);
    }
}
