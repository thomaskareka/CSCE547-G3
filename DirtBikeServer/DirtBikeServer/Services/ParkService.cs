using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class ParkService : IParkService {
        private readonly IParkRepository _repository;
        public ParkService(IParkRepository repository) => _repository = repository;

    public async Task<bool> AddGuestLimitToPark(ParkDTOs.GuestDTO dto)
    {
      if (dto.ParkId == Guid.Empty)
        throw new ArgumentException("Park ID cannot be empty.", nameof(dto.ParkId));

      if (dto.NumberOfGuests <= 0)
        throw new ArgumentException("Guest limit must be greater than zero.", nameof(dto.NumberOfGuests));

      // Get the park from the repository
      var park = await _repository.GetParkFromIdAsync(dto.ParkId);
      if (park == null)
        throw new ParkNotFoundException(dto.ParkId);

      // Actually set the limit
      park.GuestLimit = dto.NumberOfGuests;

      // Save changes
      return await _repository.UpdateParkAsync(park);
    }


    public async Task<bool> AddPark(ParkDTOs.CreateParkDTO dto) {
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Park name cannot be empty.", nameof(dto.Name));

            if (string.IsNullOrWhiteSpace(dto.Location))
                throw new ArgumentException("Park location cannot be empty.", nameof(dto.Location));

            
            var park = new Park {
                Name = dto.Name,
                Location = dto.Location,
                Description = dto.Description,
                GuestLimit = dto.GuestLimit,
                AdultPrice = dto.AdultPrice,
                ChildPrice = dto.ChildPrice
            };
            return await _repository.AddParkAsync(park);
        }

        public async Task<bool> EditPark(ParkDTOs.EditParkDTO dto) {
            if (dto.ParkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(dto.ParkId));

            var park = await _repository.GetParkFromIdAsync(dto.ParkId);
            if (park == null) {
                throw new ParkNotFoundException(dto.ParkId);
            }

            if (dto.Description is not null)
                park.Description = dto.Description;

            if (dto.Name is not null)
                park.Name = dto.Name;

            if (dto.AdultPrice.HasValue)
                park.AdultPrice = dto.AdultPrice.Value;

            if (dto.ChildPrice.HasValue)
                park.ChildPrice = dto.ChildPrice.Value;

            if (dto.GuestLimit.HasValue)
                park.GuestLimit = dto.GuestLimit.Value;

            return await _repository.UpdateParkAsync(park);
        }

        public async Task<Park?> GetPark(Guid parkId) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));
            var park = await _repository.GetParkFromIdAsync(parkId);
            if (park == null) {
                throw new ParkNotFoundException(parkId);
            }
            return park;

        }

        public async Task<List<Park>> GetParks() {
            return await _repository.GetParksAsync();
        }

        public Task<bool> RemoveGuestsFromPark(ParkDTOs.GuestDTO dto) {
            if (dto.ParkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(dto.ParkId));

            if (dto.NumberOfGuests <= 0)
                throw new ArgumentException("Number of guests to remove must be greater than zero.", nameof(dto.NumberOfGuests));

            throw new NotImplementedException();
        }

        public async Task<bool> RemovePark(Guid parkId) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));
            return await _repository.DeleteParkFromIdAsync(parkId);
        }
    }
}
