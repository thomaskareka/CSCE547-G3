using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DirtBikeServer.Data {
    public class BookingRepository: IBookingRepository {
        private readonly DirtBikeDbContext _context;
        public BookingRepository(DirtBikeDbContext context) => _context = context;

        public async Task<bool> AddBookingAsync(Booking booking) {
            _context.Bookings.Add(booking);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<List<Booking>> GetBookingsAsync() {
            return await _context.Bookings.ToListAsync();
        }

        public async Task<Booking?> GetBookingFromIdAsync(Guid id) {
            var query = from booking in _context.Bookings
                        where booking.Id == id
                        select booking;
            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> DeleteBookingFromIdAsync(Guid id) {
            var query = from booking in _context.Bookings
                        where booking.Id == id
                        select booking;
            var bookingToDelete = await query.FirstOrDefaultAsync();

            if (bookingToDelete == null)
                return false;
            _context.Bookings.Remove(bookingToDelete);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
