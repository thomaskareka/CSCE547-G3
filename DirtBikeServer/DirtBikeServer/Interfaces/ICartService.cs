using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface ICartService {
        Task<bool> AddBookingToCart(Guid cartId, Guid parkId, Booking bookingInfo);
        Task<Cart> RemoveBookingFromCart(Guid cartId, Guid bookingId);
        Task<Cart> GetCart(Guid cartId);
        Task<Cart> GetCart();
        Task<bool> ProcessPayment(Guid cartId, string cardNumber, DateTime exp, string cardHolderName, int cvc);
    }
}
