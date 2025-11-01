using DirtBikeServer.Interfaces;

namespace DirtBikeServer.Data {
    public class BookingRepository: IBookingRepository {
        private readonly DirtBikeDbContext _context;
    }
}
