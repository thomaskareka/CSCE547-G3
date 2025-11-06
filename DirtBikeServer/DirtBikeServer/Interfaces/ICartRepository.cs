using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface ICartRepository {
        Task<Cart?> GetCartAsync(Guid cartId);
        Task<bool> CreateCartAsync(Cart cartId);

        Task<bool> AddBookingAsync(Cart cart, Booking booking);
        Task<bool> RemoveBookingAsync(Cart cart, Booking booking);
    }
}
