namespace DirtBikeServer.Models {
    public class Cart {
        public Guid Id { get; set; } = Guid.NewGuid();
        public virtual ICollection<Booking> Items { get; set; } = new List<Booking>();
    }
}
