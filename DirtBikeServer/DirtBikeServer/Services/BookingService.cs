using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {

    // Service responsible for booking-related business logic.
    // Acts as a middle layer between controllers and repositories.
    public class BookingService : IBookingService {

        private readonly IBookingRepository _repository;   // Data-access abstraction for bookings

        // Dependency Injection: repository is provided by the DI container
        public BookingService(IBookingRepository repository) => _repository = repository;

        // Retrieves a single booking by ID with validation
        public async Task<Booking?> GetBooking(Guid bookingId) {
            // Validate that the provided ID is not empty
            if (bookingId == Guid.Empty)
                throw new ArgumentException("Invalid booking ID.", nameof(bookingId));

            // Query repository for the booking
            return await _repository.GetBookingFromIdAsync(bookingId);
        }

        // Returns a list of all bookings from the database
        public async Task<List<Booking>> GetBookings() {
            return await _repository.GetBookingsAsync();
        }

        // Removes a booking if it exists, returns true/false depending on success
        public async Task<bool> RemoveBooking(Guid bookingId) {
            // Validation: ensure bookingId is meaningful
            if (bookingId == Guid.Empty)
                throw new ArgumentException("Invalid booking ID.", nameof(bookingId));

            // Confirm that the booking exists
            var existing = await _repository.GetBookingFromIdAsync(bookingId);
            if (existing == null)
                return false;   // Caller interprets false as "not found"

            // Delegate to repository to delete booking
            return await _repository.DeleteBookingFromIdAsync(bookingId);
        }

        // Creates a new booking for a specific park with strong input validation
        public async Task<bool> CreateBooking(Guid parkId, BookingDTOs.CreateBookingDTO dto) {

            // Validation: park ID must not be empty
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            // Adults/children must be non-negative
            if (dto.Adults < 0 || dto.Children < 0)
                throw new ArgumentException("Adults and children cannot be negative.");

            // At least one person must be included in the booking
            if (dto.Adults + dto.Children == 0)
                throw new ArgumentException("At least one participant is required.");

            // If a cart ID was provided, ensure it is not Guid.Empty
            if (dto.CartId.HasValue && dto.CartId == Guid.Empty)
                throw new ArgumentException("Invalid cart ID.", nameof(dto.CartId));

            // Construct domain model object from DTO
            var booking = new Booking {
                ParkId = parkId,
                Adults = dto.Adults,
                Children = dto.Children,
                CartID = dto.CartId,
                StartDate = dto.StartDate,
                NumDays = dto.NumDays
            };

            // Pass to repository for persistence, return success indicator
            return await _repository.AddBookingAsync(booking);
        }
    }
}
