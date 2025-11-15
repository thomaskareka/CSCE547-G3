using DirtBikeServer.Models;
using Microsoft.EntityFrameworkCore;

namespace DirtBikeServer.Data {
    public class DirtBikeDbContext: DbContext {
        public DirtBikeDbContext(DbContextOptions<DirtBikeDbContext> options) : base(options) { }

        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Park> Parks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<Park>()
                .Property(p => p.AdultPrice)
                .HasPrecision(8, 2);
            modelBuilder.Entity<Park>()
                .Property(p => p.ChildPrice)
                .HasPrecision(8, 2);
            
            base.OnModelCreating(modelBuilder);
        }
    }
}
