using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface IBookingService {
        Task<Booking> GetBooking(int parkId);
        Task<List<Booking>> GetBookings();
        Task<bool> RemoveBooking(int bookingId);
        Task<bool> CreateBooking(int parkId);
    }
}
