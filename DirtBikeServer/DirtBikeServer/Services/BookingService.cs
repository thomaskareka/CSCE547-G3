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

        public async Task<bool> CreateBooking(Guid parkId, BookingDTOs.CreateBookingDTO dto) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            if (dto.Adults < 0 || dto.Children < 0)
                throw new ArgumentException("Adults and children cannot be negative.");

            if (dto.Adults + dto.Children == 0)
                throw new ArgumentException("At least one participant is required.");

            if (dto.CartId.HasValue && dto.CartId == Guid.Empty)
                throw new ArgumentException("Invalid cart ID.", nameof(dto.CartId));

            var booking = new Booking {
                ParkId = parkId,
                Adults = dto.Adults,
                Children = dto.Children,
                CartID = dto.CartId
            };
            return await _repository.AddBookingAsync(booking);
        }
    }
}
