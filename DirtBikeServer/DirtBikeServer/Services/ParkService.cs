using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class ParkService : IParkService {
        private readonly IParkRepository _repository;
        public ParkService(IParkRepository repository) => _repository = repository;

        public Task<bool> AddGuestLimitToPark(Guid parkId, int numberOfGuests) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            if (numberOfGuests <= 0)
                throw new ArgumentException("Guest limit must be greater than zero.", nameof(numberOfGuests));
            throw new NotImplementedException();
        }

        public async Task<bool> AddPark(string name, string location, string? description) {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Park name cannot be empty.", nameof(name));

            if (string.IsNullOrWhiteSpace(location))
                throw new ArgumentException("Park location cannot be empty.", nameof(location));

            
            var park = new Park {
                Name = name,
                Location = location,
                Description = description
            };
            return await _repository.AddParkAsync(park);
        }

        public Task<bool> EditPark(Guid parkId, Park newPark) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            if (newPark == null)
                throw new ArgumentNullException(nameof(newPark));

            if (string.IsNullOrWhiteSpace(newPark.Name))
                throw new ArgumentException("Park name cannot be empty.", nameof(newPark.Name));

            if (string.IsNullOrWhiteSpace(newPark.Location))
                throw new ArgumentException("Park location cannot be empty.", nameof(newPark.Location));

            throw new NotImplementedException();
        }

        public async Task<Park?> GetPark(Guid parkId) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));
            return await _repository.GetParkFromIdAsync(parkId);
        }

        public async Task<List<Park>> GetParks() {
            return await _repository.GetParksAsync();
        }

        public Task<bool> RemoveGuestsFromPark(Guid parkId, int numberOfGuests) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            if (numberOfGuests <= 0)
                throw new ArgumentException("Number of guests to remove must be greater than zero.", nameof(numberOfGuests));

            throw new NotImplementedException();
        }

        public async Task<bool> RemovePark(Guid parkId) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));
            return await _repository.DeleteParkFromIdAsync(parkId);
        }
    }
}
