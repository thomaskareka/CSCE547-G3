using DirtBikeServer.Models;
using System;

namespace DirtBikeServer.Data {
    public class ParkDTOs {
        public class CreateParkDTO {
            public required string Name { get; set; }
            public required string Location { get; set; }
            public string? Description { get; set; }
        }

        public class GuestDTO {
            public Guid ParkId {  get; set; }
            public int NumberOfGuests { get; set; }
        }

        public class EditParkDTO {
            public Guid ParkId { get; set; }
            public decimal AdultPrice { get; set; }
            public decimal ChildPrice { get; set; }
            public int GuestLimit { get; set; }
        }
    }
}
