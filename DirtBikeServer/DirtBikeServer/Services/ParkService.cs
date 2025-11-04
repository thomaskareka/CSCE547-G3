using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class ParkService : IParkService {
        private readonly IParkRepository _repository;
        public ParkService(IParkRepository repository) => _repository = repository;

        public Task<bool> AddGuestLimitToPark(Guid parkId, int numberOfGuests) {
            throw new NotImplementedException();
        }

        public async Task<bool> AddPark(string name, string location, string? description) {
            var park = new Park {
                Name = name,
                Location = location,
                Description = description
            };
            return await _repository.AddParkAsync(park);
        }

        public Task<bool> EditPark(Guid parkId, Park newPark) {
            throw new NotImplementedException();
        }

        public async Task<Park?> GetPark(Guid parkId) {
            return await _repository.GetParkFromIdAsync(parkId);
        }

        public async Task<List<Park>> GetParks() {
            return await _repository.GetParksAsync();
        }

        public Task<bool> RemoveGuestsFromPark(Guid parkId, int numberOfGuests) {
            throw new NotImplementedException();
        }

        public async Task<bool> RemovePark(Guid parkId) {
            return await _repository.DeleteParkFromIdAsync(parkId);
        }
    }
}
