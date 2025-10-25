using DirtBikeServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DirtBikeServer.Data {
    public class DirtBikeDbContext: DbContext {
        public DirtBikeDbContext(DbContextOptions<DirtBikeDbContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Park> Parks { get; set; }
    }
}
