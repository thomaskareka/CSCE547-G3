namespace DirtBikeServer.Models {
    public class Cart {
        public Guid Id { get; set; }
        public virtual ICollection<Booking> Items { get; set; } = new List<Booking>();
    }
}
