using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class CartService : ICartService {
        private readonly ICartRepository _repository;
        public CartService(ICartRepository repository) => _repository = repository;

        public async Task<bool> AddBookingToCart(CartDTOs.CreateCartBookingDTO dto) {
            if (dto.CartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(dto.CartId));

            if (dto.ParkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(dto.ParkId));

            if (dto.BookingInfo == null)
                throw new ArgumentNullException(nameof(dto.BookingInfo), "Booking information cannot be null.");

            if (dto.BookingInfo.Adults < 0 || dto.BookingInfo.Children < 0)
                throw new ArgumentException("Adults and children must be non-negative values.");

            if (dto.BookingInfo.Adults + dto.BookingInfo.Children == 0)
                throw new ArgumentException("At least one participant is required to create a booking.");

            var cart = await _repository.GetCartAsync(dto.CartId);
            if (cart == null)
                return false;

            var booking = new Booking {
                ParkId = dto.ParkId,
                Adults = dto.BookingInfo.Adults,
                Children = dto.BookingInfo.Children,
                CartID = dto.CartId
            };

            return await _repository.AddBookingAsync(cart, booking);
        }
// First attempts to obtain a cart with the given ID.
// If the cart does not exist, it will query the database to create a new one.
// TODO: Error handling if the database cannot create the cart.
        public async Task<CartDTOs.CartResponseDTO> GetCart(Guid cartId) {
            if (cartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));
            var searchedCart = await _repository.GetCartAsync(cartId);
            if (searchedCart == null) {
                var createdCart = new Cart();
                createdCart.Id = cartId;

                var success = await _repository.CreateCartAsync(createdCart);
                if (!success)
                    throw new InvalidOperationException("Failed to create a new cart in the database.");
                var response = new CartDTOs.CartResponseDTO {
                    OutCart = createdCart,
                    Id = createdCart.Id
                };
                return response;
            } else {
                var response = new CartDTOs.CartResponseDTO {
                    OutCart = searchedCart,
                    Id = searchedCart.Id
                };
                return response;
            }
        }
// If no ID is specified, always create a new cart, then return it.
        public async Task<CartDTOs.CartResponseDTO> GetCart() {
            var cart = new Cart();
            var success = await _repository.CreateCartAsync(cart);
            if (!success)
                throw new InvalidOperationException("Failed to create a new cart in the database.");

            var response = new CartDTOs.CartResponseDTO {
                OutCart = cart,
                Id = cart.Id
            };
            return response;
        }

        public Task<bool> ProcessPayment(CartDTOs.ProcessPaymentDTO dto) {
            throw new NotImplementedException();
        }

        public async Task<Cart> RemoveBookingFromCart(CartDTOs.RemoveBookingDTO dto) {
            if (dto.CartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(dto.CartId));

            if (dto.BookingId == Guid.Empty)
                throw new ArgumentException("Booking ID cannot be empty.", nameof(dto.BookingId));

            var cart = await _repository.GetCartAsync(dto.CartId);
            if (cart == null)
                throw new KeyNotFoundException($"Cart with ID {dto.CartId} was not found.");

            var booking = cart.Items.FirstOrDefault(b => b.Id == dto.BookingId);

            if (booking == null)
                return cart;

            var success = await _repository.RemoveBookingAsync(cart, booking);
            if (!success)
                throw new InvalidOperationException("Failed to remove booking from cart.");

            return cart;
                
        }
    }
}
