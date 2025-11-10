 using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface IBookingRepository {
        Task<bool> AddBookingAsync(Booking booking);
        Task<List<Booking>> GetBookingsAsync();
        Task<Booking?> GetBookingFromIdAsync(Guid id);
        Task<bool> DeleteBookingFromIdAsync(Guid id);
    }
}
