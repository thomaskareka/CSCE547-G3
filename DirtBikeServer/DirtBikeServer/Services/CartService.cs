using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {

    // Service layer implementing business logic related to Cart operations.
    // Acts as the middle layer between controllers and repositories.
    public class CartService : ICartService {

        private readonly ICartRepository _repository;       // Handles cart + booking persistence
        private readonly IParkRepository _parkRepository;   // Used for guest-limit checks

        // Constructor injection of required repositories
        public CartService(ICartRepository repository, IParkRepository parkRepository) {
            _repository = repository;
            _parkRepository = parkRepository;
        }

        // Adds a booking into an existing cart after validating booking input
        // and ensuring park guest limits are not exceeded for *any* day of the booking.
        public async Task<bool> AddBookingToCart(CartDTOs.CreateCartBookingDTO dto) {

            // Basic input validation
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

            // Protect against unrealistic or negative booking durations
            if (dto.BookingInfo.NumDays > 7 || dto.BookingInfo.NumDays <= 0)
                throw new ArgumentException("Invalid length.");

            // Retrieve or fail if cart does not exist
            var cart = await _repository.GetCartAsync(dto.CartId);
            if (cart == null)
                return false;

            // Fetch the maximum allowed guest limit for the park
            var guestLimit = await _parkRepository.GetParkBookingLimit(dto.ParkId);
            var guestsToAdd = dto.BookingInfo.Adults + dto.BookingInfo.Children;

            // For multi-day bookings, ensure guest limit holds for each day
            for (int i = 0; i < dto.BookingInfo.NumDays; i++) {
                var dateToCheck = dto.BookingInfo.StartDate.Date.AddDays(i);

                // Count how many guests are already booked for this date
                var bookingCountForDay = await _parkRepository.GetNumberOfBookingsForDay(dto.ParkId, dateToCheck);

                // If adding this booking would exceed capacity, reject the request
                if (guestLimit < bookingCountForDay + guestsToAdd) {
                    throw new ParkFullException(dto.ParkId, dateToCheck);
                }
            }

            // Create domain model object from DTO
            var booking = new Booking {
                ParkId = dto.ParkId,
                Adults = dto.BookingInfo.Adults,
                Children = dto.BookingInfo.Children,
                CartID = dto.CartId,
                StartDate = dto.BookingInfo.StartDate,
                NumDays = dto.BookingInfo.NumDays
            };

            // Add booking through repository, associating it with the cart
            return await _repository.AddBookingAsync(cart, booking);
        }

        // Fetches an existing cart or creates a new one if it doesn't exist.
        // Used when the cart ID is known.
        public async Task<CartDTOs.CartResponseDTO> GetCart(Guid cartId) {
            if (cartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(cartId));

            var searchedCart = await _repository.GetCartAsync(cartId);

            // If cart exists, return it wrapped in a DTO
            if (searchedCart != null) {
                return new CartDTOs.CartResponseDTO {
                    OutCart = searchedCart,
                    Id = searchedCart.Id
                };
            }

            // If not found, create a new cart with the supplied ID
            var createdCart = new Cart { Id = cartId };
            var success = await _repository.CreateCartAsync(createdCart);

            if (!success)
                throw new InvalidOperationException("Failed to create a new cart in the database.");

            return new CartDTOs.CartResponseDTO {
                OutCart = createdCart,
                Id = createdCart.Id
            };
        }

        // Creates a new cart every time it is called—no ID supplied.
        public async Task<CartDTOs.CartResponseDTO> GetCart() {
            var cart = new Cart();

            var success = await _repository.CreateCartAsync(cart);
            if (!success)
                throw new InvalidOperationException("Failed to create a new cart in the database.");

            return new CartDTOs.CartResponseDTO {
                OutCart = cart,
                Id = cart.Id
            };
        }

        // Validates and processes a payment, then finalizes the bookings.
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

            // Validate credit card number using Luhn's algorithm
            var valid = CardValid(dto.CardNumber);
            if (!valid)
                throw new InvalidCardException("Number");

            // TODO: Recheck park guest limits in case slots filled since adding to cart

            // Finalize bookings: mark them as purchased by clearing cart references
            foreach (Booking booking in searchedCart.Items) {
                booking.CartID = null;
            }

            // Commit all changes to database
            return await _repository.SaveChangesAsync(searchedCart);
        }

        // Luhn's algorithm implementation for credit card validation
        private static bool CardValid(string number) {
            if (string.IsNullOrWhiteSpace(number))
                return false;

            if (!number.All(char.IsDigit))
                return false;

            int sum = 0;
            bool alternate = false;

            // Iterate digits from right to left
            for (int i = number.Length - 1; i >= 0; i--) {

                int n = number[i] - '0';  // Convert character digit to int

                if (alternate) {
                    n *= 2;
                    if (n > 9)
                        n -= 9;
                }

                sum += n;
                alternate = !alternate;
            }

            // Luhn requirement: total sum must be divisible by 10
            return (sum % 10 == 0);
        }

        // Removes a booking from a cart and returns the updated cart.
        public async Task<Cart> RemoveBookingFromCart(CartDTOs.RemoveBookingDTO dto) {

            if (dto.CartId == Guid.Empty)
                throw new ArgumentException("Cart ID cannot be empty.", nameof(dto.CartId));

            if (dto.BookingId == Guid.Empty)
                throw new ArgumentException("Booking ID cannot be empty.", nameof(dto.BookingId));

            // Retrieve the cart containing the booking
            var cart = await _repository.GetCartAsync(dto.CartId);
            if (cart == null)
                throw new KeyNotFoundException($"Cart with ID {dto.CartId} was not found.");

            // Locate the booking inside the cart
            var booking = cart.Items.FirstOrDefault(b => b.Id == dto.BookingId);

            // If booking is not inside the cart, return the unchanged cart
            if (booking == null)
                return cart;

            // Remove and persist the removal
            var success = await _repository.RemoveBookingAsync(cart, booking);

            if (!success)
                throw new InvalidOperationException("Failed to remove booking from cart.");

            return cart;
        }
    }
}
