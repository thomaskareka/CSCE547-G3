using DirtBikeServer.Models;

namespace DirtBikeServer.Data {
    public class CartDTOs {
        public class CreateCartBookingDTO {
            public Guid CartId { get; set; }
            public Guid ParkId { get; set; }
            public required BookingDTOs.BookingInfoDTO BookingInfo { get; set; }
        }

        public class CartResponseDTO {

            public Cart? OutCart { get; set; }
            public Guid Id { get; set; }
        }

        public class RemoveBookingDTO {
            public Guid CartId { get; set; }
            public Guid BookingId { get; set; }
        }

        public class ProcessPaymentDTO {
            public Guid CartId { get; set; }
            public required string CardNumber { get; set; }
            public DateTime ExpirationDate { get; set; }
            public required string CardHolderName { get; set; }
            public int Cvc {  get; set; }
        }
    }
}
