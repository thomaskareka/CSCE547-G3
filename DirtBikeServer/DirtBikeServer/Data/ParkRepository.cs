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

        public async Task<List<Park>> GetParksAsync() {
            return await _context.Parks.ToListAsync();
        }
    }
}
