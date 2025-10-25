using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface ICartService {
        Task<bool> AddBookingToCart(int cartId, int parkId, Booking bookingInfo);
        Task<Cart> RemoveBookingFromCart(int cartId, int bookingId);
        Task<Cart> GetCart(int cartId);
        Task<Cart> GetCart();
        Task<bool> ProcessPayment(int cartId, string cardNumber, DateTime exp, string cardHolderName, int cvc);
    }
}
