using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class ParkService : IParkService {
        private readonly IParkRepository _repository;
        public ParkService(IParkRepository repository) => _repository = repository;

        public Task<bool> AddGuestLimitToPark(Park park, int numberOfGuests) {
            throw new NotImplementedException();
        }

        public async Task<bool> AddPark(Park park) {
            return await _repository.AddParkAsync(park);
        }

        public Task<bool> EditPark(Park park, Park newPark) {
            throw new NotImplementedException();
        }

        public Task<Park> GetPark(Guid parkId) {
            throw new NotImplementedException();
        }

        public async Task<List<Park>> GetParks() {
            return await _repository.GetParksAsync();
        }

        public Task<bool> RemoveGuestsFromPark(Park park, int numberOfGuests) {
            throw new NotImplementedException();
        }

        public Task<bool> RemovePark(Guid parkId) {
            throw new NotImplementedException();
        }
    }
}
