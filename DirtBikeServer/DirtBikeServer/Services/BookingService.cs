using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class BookingService: IBookingService {
        private readonly DirtBikeDbContext _context;
        public BookingService(DirtBikeDbContext context) => _context = context;

        public Task<Booking> GetBooking(Guid parkId) {
            throw new NotImplementedException();
        }

        public Task<List<Booking>> GetBookings() {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveBooking(Guid bookingId) {
            throw new NotImplementedException();
        }

        public Task<bool> CreateBooking(Guid parkId) {
            throw new NotImplementedException();
        }
    }
}
