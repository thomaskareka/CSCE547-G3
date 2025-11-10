using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class BookingService: IBookingService {
        private readonly IBookingRepository _repository;
        public BookingService(IBookingRepository repository) => _repository = repository;

        public async Task<Booking?> GetBooking(Guid bookingId) {
            if (bookingId == Guid.Empty)
                throw new ArgumentException("Invalid booking ID.", nameof(bookingId));
            return await _repository.GetBookingFromIdAsync(bookingId);
        }

        public async Task<List<Booking>> GetBookings() {
            return await _repository.GetBookingsAsync();
        }

        public async Task<bool> RemoveBooking(Guid bookingId) {
            if (bookingId == Guid.Empty)
                throw new ArgumentException("Invalid booking ID.", nameof(bookingId));

            var existing = await _repository.GetBookingFromIdAsync(bookingId);
            if (existing == null)
                return false;
            return await _repository.DeleteBookingFromIdAsync(bookingId);
        }

        public async Task<bool> CreateBooking(Guid parkId, int adults, int children, Guid? cartId) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            if (adults < 0 || children < 0)
                throw new ArgumentException("Adults and children cannot be negative.");

            if (adults + children == 0)
                throw new ArgumentException("At least one participant is required.");

            if (cartId.HasValue && cartId == Guid.Empty)
                throw new ArgumentException("Invalid cart ID.", nameof(cartId));

            var booking = new Booking {
                ParkId = parkId,
                Adults = adults,
                Children = children,
                CartID = cartId
            };
            return await _repository.AddBookingAsync(booking);
        }
    }
}
