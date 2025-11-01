using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class ParkService : IParkService {
        private readonly DirtBikeDbContext _context;
        public ParkService(DirtBikeDbContext context) => _context = context;

        public Task<bool> AddGuestLimitToPark(Park park, int numberOfGuests) {
            throw new NotImplementedException();
        }

        public Task<bool> AddPark(Park park) {
            throw new NotImplementedException();
        }

        public Task<bool> EditPark(Park park, Park newPark) {
            throw new NotImplementedException();
        }

        public Task<Park> GetPark(Guid parkId) {
            throw new NotImplementedException();
        }

        public Task<List<Park>> GetParks() {
            throw new NotImplementedException();
        }

        public Task<bool> RemoveGuestsFromPark(Park park, int numberOfGuests) {
            throw new NotImplementedException();
        }

        public Task<bool> RemovePark(Guid parkId) {
            throw new NotImplementedException();
        }
    }
}
