using DirtBikeServer.Models;
using DirtBikeServer.Data;

namespace DirtBikeServer.Interfaces {
    public interface ICartService {
        Task<bool> AddBookingToCart(Guid cartId, Guid parkId, Booking bookingInfo);
        Task<Cart> RemoveBookingFromCart(Guid cartId, Guid bookingId);
        Task<CartDTOs.CartResponseDTO> GetCart(Guid cartId);
        Task<CartDTOs.CartResponseDTO> GetCart();
        Task<bool> ProcessPayment(Guid cartId, string cardNumber, DateTime exp, string cardHolderName, int cvc);
    }
}
