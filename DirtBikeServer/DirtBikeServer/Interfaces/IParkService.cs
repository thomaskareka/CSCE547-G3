using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface IParkService {
        Task<bool> AddPark(string name, string location, string? description);
        Task<bool> RemovePark(Guid parkId);
        Task<Park?> GetPark(Guid parkId);
        Task<List<Park>> GetParks();
        Task<bool> AddGuestLimitToPark(Guid parkId, int numberOfGuests);
        Task<bool> RemoveGuestsFromPark(Guid parkId, int numberOfGuests);
        Task<bool> EditPark(Guid parkId, Park newPark);
    }
}
