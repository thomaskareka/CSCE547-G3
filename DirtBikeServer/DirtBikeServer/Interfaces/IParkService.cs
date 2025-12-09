using DirtBikeServer.Data;
using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface IParkService {
        Task<bool> AddPark(ParkDTOs.CreateParkDTO dto);
        Task<bool> RemovePark(Guid parkId);
        Task<Park?> GetPark(Guid parkId);
        Task<List<Park>> GetParks();
        Task<bool> AddGuestLimitToPark(ParkDTOs.GuestDTO dto);
        Task<bool> RemoveGuestsFromPark(ParkDTOs.RemoveGuestDTO dto);
        Task<bool> EditPark(ParkDTOs.EditParkDTO dto);
    }
}
