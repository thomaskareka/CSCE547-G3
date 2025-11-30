using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DirtBikeServer.Data {
    public class ParkRepository: IParkRepository {
        private readonly DirtBikeDbContext _context;
        public ParkRepository(DirtBikeDbContext context) => _context = context;

        public async Task<bool> AddParkAsync(Park park) {
            _context.Parks.Add(park);
            return await _context.SaveChangesAsync() > 0;
            
        }

        public async Task<Park?> GetParkFromIdAsync(Guid id) {
            var query = from park in _context.Parks
                        where park.Id == id
                        select park;
            return await query.FirstOrDefaultAsync();
        }

        public async Task<List<Park>> GetParksAsync() {
            return await _context.Parks.ToListAsync();
        }

        public async Task<bool> DeleteParkFromIdAsync(Guid id) {
            var query = from park in _context.Parks
                        where park.Id == id
                        select park;
            var parkToDelete = await query.FirstOrDefaultAsync();

            if (parkToDelete == null)
                throw new ParkNotFoundException(id);

            _context.Parks.Remove(parkToDelete);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetNumberOfBookingsForDay(Guid parkId, DateTime? day) {
            if (day == null)
                day = DateTime.Today;
            var park = await GetParkFromIdAsync(parkId);
            if (park == null)
                return 0;
            var query = from booking in park.Bookings
                        where day >= booking.StartDate
                            && day < booking.StartDate.AddDays(booking.NumDays)
                            && booking.CartID == null
                        select booking.Adults + booking.Children;

            return query.Sum();
        }
        public async Task<bool> UpdateParkAsync(Park park) {
            _context.Update(park);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<int> GetParkBookingLimit(Guid parkId) {
            var park = await GetParkFromIdAsync(parkId);
            if (park == null) return 0;
            return park.GuestLimit;
        }
    }
}
