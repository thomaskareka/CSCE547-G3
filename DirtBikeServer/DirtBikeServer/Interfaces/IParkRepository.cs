using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface IParkRepository {
        Task<bool> AddParkAsync(Park park);
        Task<List<Park>> GetParksAsync();
    }
}