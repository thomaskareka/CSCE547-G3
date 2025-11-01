using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class CartService : ICartService {
        private readonly DirtBikeDbContext _context;
        public CartService(DirtBikeDbContext context) => _context = context;

        public Task<bool> AddBookingToCart(Guid cartId, Guid parkId, Booking bookingInfo) {
            throw new NotImplementedException();
        }

        public Task<Cart> GetCart(Guid cartId) {
            throw new NotImplementedException();
        }

        public Task<Cart> GetCart() {
            throw new NotImplementedException();
        }

        public Task<bool> ProcessPayment(Guid cartId, string cardNumber, DateTime exp, string cardHolderName, int cvc) {
            throw new NotImplementedException();
        }

        public Task<Cart> RemoveBookingFromCart(Guid cartId, Guid bookingId) {
            throw new NotImplementedException();
        }
    }
}
