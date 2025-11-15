using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface IParkRepository {
        Task<bool> AddParkAsync(Park park);
        Task<List<Park>> GetParksAsync();
        Task<Park?> GetParkFromIdAsync(Guid id);
        Task<bool> DeleteParkFromIdAsync(Guid parkId);
        Task<int> GetNumberOfBookingsForDay(Guid parkId,  DateTime? day);
    }
}