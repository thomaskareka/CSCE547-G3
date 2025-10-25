using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface IParkService {
        Task<bool> AddPark(Park park);
        Task<bool> RemovePark(int parkId);
        Task<Park> GetPark(int parkId);
        Task<List<Park>> GetParks();
        Task<bool> AddGuestLimitToPark(Park park, int numberOfGuests);
        Task<bool> RemoveGuestsFromPark(Park park, int numberOfGuests);
        Task<bool> EditPark(Park park, Park newPark);
    }
}
