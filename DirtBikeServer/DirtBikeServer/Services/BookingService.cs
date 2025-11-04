using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class BookingService: IBookingService {
        private readonly IBookingRepository _repository;
        public BookingService(IBookingRepository repository) => _repository = repository;

        public async Task<Booking?> GetBooking(Guid bookingId) {
            return await _repository.GetBookingFromIdAsync(bookingId);
        }

        public async Task<List<Booking>> GetBookings() {
            return await _repository.GetBookingsAsync();
        }

        public async Task<bool> RemoveBooking(Guid bookingId) {
            return await _repository.DeleteBookingFromIdAsync(bookingId);
        }

        public async Task<bool> CreateBooking(Guid parkId, int adults, int children, Guid? cartId) {
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
