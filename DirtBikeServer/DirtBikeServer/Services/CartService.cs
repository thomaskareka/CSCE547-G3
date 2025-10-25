using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;

namespace DirtBikeServer.Services {
    public class CartService : ICartService {
        private readonly DirtBikeDbContext _context;
        public CartService(DirtBikeDbContext context) => _context = context;

        public Task<bool> AddBookingToCart(int cartId, int parkId, Booking bookingInfo) {
            throw new NotImplementedException();
        }

        public Task<Cart> GetCart(int cartId) {
            throw new NotImplementedException();
        }

        public Task<Cart> GetCart() {
            throw new NotImplementedException();
        }

        public Task<bool> ProcessPayment(int cartId, string cardNumber, DateTime exp, string cardHolderName, int cvc) {
            throw new NotImplementedException();
        }

        public Task<Cart> RemoveBookingFromCart(int cartId, int bookingId) {
            throw new NotImplementedException();
        }
    }
}
