using DirtBikeServer.Interfaces;

namespace DirtBikeServer.Data {
    public class BookingRepository: IBookingRepository {
        private readonly DirtBikeDbContext _context;
        public BookingRepository(DirtBikeDbContext context) => _context = context;
    }
}
