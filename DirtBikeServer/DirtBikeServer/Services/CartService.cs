using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class CartService : ICartService {
        private readonly ICartRepository _repository;
        // CartService relies on certain read-only calls from the park repository.
        // To avoid duplicating helper methods, the cart service also has access to the park repository.
        private readonly IParkRepository _parkRepository;
        public CartService(ICartRepository repository, IParkRepository parkRepository) {
            _repository = repository;
            _parkRepository = parkRepository;
        }

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

            if (dto.BookingInfo.NumDays > 7 || dto.BookingInfo.NumDays <= 0)
                throw new ArgumentException("Invalid length.");

            var cart = await _repository.GetCartAsync(dto.CartId);
            if (cart == null)
                return false;

            // query the park repository to ensure that the booking fulfills the park limit
            var guestLimit = await _parkRepository.GetParkBookingLimit(dto.ParkId);
            var guestsToAdd = dto.BookingInfo.Adults + dto.BookingInfo.Children;

            // TODO: better logging/return, optimize to use less queries
            for (int i = 0; i < dto.BookingInfo.NumDays; i++) {
                var dateToCheck = dto.BookingInfo.StartDate.Date.AddDays(i);
                var bookingCountForDay = await _parkRepository.GetNumberOfBookingsForDay(dto.ParkId, dateToCheck);

                if(guestLimit < bookingCountForDay + guestsToAdd) {
                    throw new ParkFullException(dto.ParkId, dateToCheck);
                }
            }

            var booking = new Booking {
                ParkId = dto.ParkId,
                Adults = dto.BookingInfo.Adults,
                Children = dto.BookingInfo.Children,
                CartID = dto.CartId,
                StartDate = dto.BookingInfo.StartDate,
                NumDays = dto.BookingInfo.NumDays
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

        public async Task<bool> ProcessPayment(CartDTOs.ProcessPaymentDTO dto) {
            if (dto.CartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(dto.CartId));
            var searchedCart = await _repository.GetCartAsync(dto.CartId);
            if (searchedCart == null)
                throw new CartNotFoundException(dto.CartId);
            if (searchedCart.Items.Count == 0)
                throw new ArgumentException("Cart must contain bookings.");
            if (dto.ExpirationDate < DateTime.UtcNow)
                throw new InvalidCardException(nameof(dto.ExpirationDate));

            // Use Luhn's algorithm to determine whether the card is valid.
            var valid = CardValid(dto.CardNumber);

            if (!valid)
                throw new InvalidCardException("Number");

            // TODO: verify that limits haven't been exceeded since adding to cart
;           
            foreach (Booking booking in searchedCart.Items) {
                booking.CartID = null;
            }
            return await _repository.SaveChangesAsync(searchedCart);
        }
        private static bool CardValid(string number) {
            if (string.IsNullOrWhiteSpace(number)) 
                return false;
            if (!number.All(char.IsDigit))
                return false;

            int sum = 0;
            bool alternate = false;

            for (int i = number.Length - 1; i >= 0; i--) {
                // subtracting ASCII value results in integer value
                int n = number[i] - '0';

                if (alternate) {
                    n *= 2;
                    if (n > 9)
                        n -= 9;
                }

                sum += n;
                alternate = !alternate;
            }
            return (sum % 10 == 0);
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
