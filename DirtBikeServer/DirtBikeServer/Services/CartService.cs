using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class CartService : ICartService {
        private readonly ICartRepository _repository;
        public CartService(ICartRepository repository) => _repository = repository;

        public async Task<bool> AddBookingToCart(Guid cartId, Guid parkId, BookingDTOs.BookingInfoDTO bookingInfo) {
            if (cartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));

            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            if (bookingInfo == null)
                throw new ArgumentNullException(nameof(bookingInfo), "Booking information cannot be null.");

            if (bookingInfo.Adults < 0 || bookingInfo.Children < 0)
                throw new ArgumentException("Adults and children must be non-negative values.");

            if (bookingInfo.Adults + bookingInfo.Children == 0)
                throw new ArgumentException("At least one participant is required to create a booking.");

            var cart = await _repository.GetCartAsync(cartId);
            if (cart == null)
                return false;

            var booking = new Booking {
                ParkId = parkId,
                Adults = bookingInfo.Adults,
                Children = bookingInfo.Children,
                CartID = cartId
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

        public Task<bool> ProcessPayment(Guid cartId, string cardNumber, DateTime exp, string cardHolderName, int cvc) {
            throw new NotImplementedException();
        }

        public async Task<Cart> RemoveBookingFromCart(Guid cartId, Guid bookingId) {
            if (cartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));

            if (bookingId == Guid.Empty)
                throw new ArgumentException("Booking ID cannot be empty.", nameof(bookingId));

            var cart = await _repository.GetCartAsync(cartId);
            if (cart == null)
                throw new KeyNotFoundException($"Cart with ID {cartId} was not found.");

            var booking = cart.Items.FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
                return cart;

            var success = await _repository.RemoveBookingAsync(cart, booking);
            if (!success)
                throw new InvalidOperationException("Failed to remove booking from cart.");

            return cart;
                
        }
    }
}
