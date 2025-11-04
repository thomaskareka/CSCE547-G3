namespace DirtBikeServer.Models {
    public class Park {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = String.Empty;
        public string Location { get; set; } = String.Empty;
        public string? Description { get; set; } 
        public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
