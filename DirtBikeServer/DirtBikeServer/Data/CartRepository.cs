using DirtBikeServer.Interfaces;

namespace DirtBikeServer.Data {
    public class CartRepository: ICartRepository {
        private readonly DirtBikeDbContext _context;
    }
}
