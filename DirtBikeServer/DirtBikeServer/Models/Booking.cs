namespace DirtBikeServer.Models {
    public class Booking {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid ParkId { get; set; }
        public virtual Park? Park { get; set; }
        public int Adults { get; set; } = 0;
        public int Children { get; set; } = 0;
        public Guid? CartID { get; set; }
        public int NumDays { get; set; } = 0;

        public DateTime StartDate { get; set; } = DateTime.Today;
    }
}
