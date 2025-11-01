using DirtBikeServer.Interfaces;

namespace DirtBikeServer.Data {
    public class ParkRepository: IParkRepository {
        private readonly DirtBikeDbContext _context;
    }
}
