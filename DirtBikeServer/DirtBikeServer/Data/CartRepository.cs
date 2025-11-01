using DirtBikeServer.Interfaces;

namespace DirtBikeServer.Data {
    public class CartRepository: ICartRepository {
        private readonly DirtBikeDbContext _context;
        public CartRepository(DirtBikeDbContext context) => _context = context;

    }
}
