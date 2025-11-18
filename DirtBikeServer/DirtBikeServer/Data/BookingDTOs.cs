using DirtBikeServer.Models;

namespace DirtBikeServer.Data {
    public class BookingDTOs {
        public class CreateBookingDTO {
            public int Adults { get; set; }
            public int Children { get; set; }
            public Guid? CartId { get; set; }
            public DateTime StartDate { get; set; } = DateTime.Today;
            public int NumDays { get; set; }
        }

        public class BookingInfoDTO {
            public int Adults { get; set; }
            public int Children { get; set; }
            public DateTime StartDate { get; set; } = DateTime.Today;
            public int NumDays { get; set; }
        }
    }
}
