using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DirtBikeServer.Data {
    public class CartRepository: ICartRepository {
        private readonly DirtBikeDbContext _context;
        public CartRepository(DirtBikeDbContext context) => _context = context;

        public async Task<bool> AddBookingAsync(Cart cart, Booking booking) {
            cart.Items.Add(booking);
            _context.Bookings.Add(booking);

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> CreateCartAsync(Cart cart) {
            _context.Carts.Add(cart);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<Cart?> GetCartAsync(Guid cartId) {
            var query = from cart in _context.Carts
                        where cart.Id == cartId
                        select cart;
            return await query.FirstOrDefaultAsync();
        }

        public async Task<bool> SaveCartsAsync() {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
