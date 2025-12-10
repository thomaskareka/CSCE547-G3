using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {

    // Business logic layer for Park operations.
    // Controllers call this service, which then delegates DB work to the repository.
    public class ParkService : IParkService {

        private readonly IParkRepository _repository;   // Data access for parks

        // Constructor injection for repository dependency
        public ParkService(IParkRepository repository) => _repository = repository;

        // Adds or updates the guest limit for a specific park
        public async Task<bool> AddGuestLimitToPark(ParkDTOs.GuestDTO dto) {

            // Basic validation
            if (dto.ParkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(dto.ParkId));

            if (dto.NumberOfGuests <= 0)
                throw new ArgumentException("Guest limit must be greater than zero.", nameof(dto.NumberOfGuests));

            // Retrieve the park entity
            var park = await _repository.GetParkFromIdAsync(dto.ParkId);
            if (park == null)
                throw new ParkNotFoundException(dto.ParkId);

            // Apply the new guest limit
            park.GuestLimit = dto.NumberOfGuests;

            // Save changes
            return await _repository.UpdateParkAsync(park);
        }

        // Adds a new park to the system
        public async Task<bool> AddPark(ParkDTOs.CreateParkDTO dto) {

            // Validation safeguards for creating a park
            if (string.IsNullOrWhiteSpace(dto.Name))
                throw new ArgumentException("Park name cannot be empty.", nameof(dto.Name));

            if (string.IsNullOrWhiteSpace(dto.Location))
                throw new ArgumentException("Park location cannot be empty.", nameof(dto.Location));

            // Construct the domain model from DTO
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

        // Partially updates an existing park's fields (only fields provided in DTO)
        public async Task<bool> EditPark(ParkDTOs.EditParkDTO dto) {

            if (dto.ParkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(dto.ParkId));

            // Retrieve the existing park
            var park = await _repository.GetParkFromIdAsync(dto.ParkId);
            if (park == null)
                throw new ParkNotFoundException(dto.ParkId);

            // Update only values provided by caller
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

            // Save changes to the database
            return await _repository.UpdateParkAsync(park);
        }

        // Fetch a single park by ID
        public async Task<Park?> GetPark(Guid parkId) {

            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            var park = await _repository.GetParkFromIdAsync(parkId);

            if (park == null)
                throw new ParkNotFoundException(parkId);

            return park;
        }

        // Retrieves all parks in the system
        public async Task<List<Park>> GetParks() {
            return await _repository.GetParksAsync();
        }

        // Removes a certain number of guests from a park for a specific date.
        // This may reduce booking sizes or delete bookings entirely.
        public async Task<bool> RemoveGuestsFromPark(ParkDTOs.RemoveGuestDTO dto) {

            if (dto.ParkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(dto.ParkId));

            if (dto.NumberOfGuests <= 0)
                throw new ArgumentException("Number of guests to remove must be greater than zero.", nameof(dto.NumberOfGuests));

            // Load park with navigation properties (bookings)
            var park = await _repository.GetParkFromIdAsync(dto.ParkId);
            if (park == null)
                throw new ParkNotFoundException(dto.ParkId);

            // Select bookings active on the target date and that are finalized (CartID == null)
            var bookings = (from booking in park.Bookings
                            where dto.Date >= booking.StartDate
                               && dto.Date < booking.StartDate.AddDays(booking.NumDays)
                               && booking.CartID == null
                            select booking)
                           .OrderBy(b => b.StartDate)
                           .ToList();

            // Compute total number of guests present that day
            int totalGuests = bookings.Sum(b => b.Adults + b.Children);

            // Prevent removing more guests than actually exist
            if (dto.NumberOfGuests > totalGuests)
                throw new ArgumentException(
                    $"Number of guests to remove cannot be larger than the number of present guests. ({totalGuests})");

            // Iterate through bookings, reducing children first, then adults
            int remainingToRemove = dto.NumberOfGuests;
            var bookingsToDelete = new List<Booking>();

            foreach (var booking in bookings) {
                if (remainingToRemove <= 0)
                    break;

                int availablePeople = booking.Adults + booking.Children;
                if (availablePeople == 0)
                    continue;

                // Remove children first
                int removedChildren = Math.Min(booking.Children, remainingToRemove);
                booking.Children -= removedChildren;
                remainingToRemove -= removedChildren;

                // If still need to remove, reduce adults
                if (remainingToRemove > 0) {
                    int removedAdults = Math.Min(booking.Adults, remainingToRemove);
                    booking.Adults -= removedAdults;
                    remainingToRemove -= removedAdults;
                }

                // If booking has no remaining guests, mark it for deletion
                if (booking.Adults + booking.Children == 0)
                    bookingsToDelete.Add(booking);
            }

            // Remove empty bookings from the park's schedule
            foreach (var booking in bookingsToDelete) {
                park.Bookings.Remove(booking);
            }

            // Persist all updates
            return await _repository.UpdateParkAsync(park);
        }

        // Deletes a park by its ID
        public async Task<bool> RemovePark(Guid parkId) {

            if (parkId == Guid.Empty)
                throw new ArgumentException("Park ID cannot be empty.", nameof(parkId));

            return await _repository.DeleteParkFromIdAsync(parkId);
        }
    }
}
