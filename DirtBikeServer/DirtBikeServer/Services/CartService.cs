using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class CartService : ICartService {
        private readonly ICartRepository _repository;
        public CartService(ICartRepository repository) => _repository = repository;

        public async Task<bool> AddBookingToCart(Guid cartId, Guid parkId, BookingDTOs.BookingInfoDTO bookingInfo) {
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
            var searchedCart = await _repository.GetCartAsync(cartId);
            if (searchedCart == null) {
                var createdCart = new Cart();
                createdCart.Id = cartId;

                var success = await _repository.CreateCartAsync(createdCart);
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
            var cart = await _repository.GetCartAsync(cartId);
            if (cart == null)
                throw new DirectoryNotFoundException();

            var booking = cart.Items.FirstOrDefault(b => b.Id == bookingId);

            if (booking == null)
                return cart;

            var success = await _repository.RemoveBookingAsync(cart, booking);

            return cart;
                
        }
    }
}
