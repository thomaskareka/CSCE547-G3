namespace DirtBikeServer.Models {
    public class Booking {
        public Guid Id { get; set; }
        public Guid ParkId { get; set; }
        public virtual Park? Park { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public Guid? CartID { get; set; }
    }
}
