using DirtBikeServer.Models;
using DirtBikeServer.Data;

namespace DirtBikeServer.Interfaces {
    public interface ICartService {
        Task<bool> AddBookingToCart(CartDTOs.CreateCartBookingDTO dto);
        Task<Cart> RemoveBookingFromCart(CartDTOs.RemoveBookingDTO dto);
        Task<CartDTOs.CartResponseDTO> GetCart(Guid cartId);
        Task<CartDTOs.CartResponseDTO> GetCart();
        Task<bool> ProcessPayment(CartDTOs.ProcessPaymentDTO dto);
    }
}
