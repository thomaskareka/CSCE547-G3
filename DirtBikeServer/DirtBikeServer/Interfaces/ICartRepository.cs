using DirtBikeServer.Models;

namespace DirtBikeServer.Interfaces {
    public interface ICartRepository {
        Task<Cart?> GetCartAsync(Guid cartId);
        Task<bool> CreateCartAsync(Cart cartId);
    }
}
