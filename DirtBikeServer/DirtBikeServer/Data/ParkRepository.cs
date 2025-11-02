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
                return false;

            _context.Parks.Remove(parkToDelete);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
