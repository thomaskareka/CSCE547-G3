using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using System.Diagnostics;
using System.Runtime.Intrinsics.Arm;

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

        public async Task<bool> RemoveGuestsFromPark(ParkDTOs.RemoveGuestDTO dto) {
            if (dto.ParkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(dto.ParkId));

            if (dto.NumberOfGuests <= 0)
                throw new ArgumentException("Number of guests to remove must be greater than zero.", nameof(dto.NumberOfGuests));

            var park = await _repository.GetParkFromIdAsync(dto.ParkId);
            if (park == null) {
                throw new ParkNotFoundException(dto.ParkId);
            }
            // get all valid bookings with that date

            var bookings = (from booking in park.Bookings
                           where dto.Date >= booking.StartDate
                                && dto.Date < booking.StartDate.AddDays(booking.NumDays)
                                && booking.CartID == null
                           select booking)
                           .OrderBy(b => b.StartDate)
                           .ToList();

            int totalGuests = bookings.Sum(b => b.Adults + b.Children);

            if (dto.NumberOfGuests > totalGuests) {
                throw new ArgumentException($"Number of guests to remove cannot be larger than the number of present guests. ({totalGuests})");
            }
            //iterate through and remove bookings until target is met
            //delete a booking if it has no people
            int remainingToRemove = dto.NumberOfGuests;
            var bookingsToDelete = new List<Booking>();

            foreach (var booking in bookings) {
                if (remainingToRemove <= 0)
                    break;

                int availablePeople = booking.Adults + booking.Children;
                if (availablePeople == 0)
                    continue;

                int removedChildren = Math.Min(booking.Children, remainingToRemove);
                booking.Children -= removedChildren;
                remainingToRemove -= removedChildren;

                if (remainingToRemove > 0) {
                    int removedAdults = Math.Min(booking.Adults, remainingToRemove);
                    booking.Adults -= removedAdults;
                    remainingToRemove -= removedAdults;
                }

                if (booking.Adults + booking.Children == 0) {
                    bookingsToDelete.Add(booking);
                }
            }

            foreach (var booking in bookingsToDelete) {
                park.Bookings.Remove(booking);
            }

            return await _repository.UpdateParkAsync(park);
        }

        public async Task<bool> RemovePark(Guid parkId) {
            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));
            return await _repository.DeleteParkFromIdAsync(parkId);
        }
    }
}
