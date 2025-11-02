using DirtBikeServer.Models;
using System;

namespace DirtBikeServer.Data {
    public class ParkDTOs {
        public class GuestDTO {
            public Guid ParkId {  get; set; }
            public int NumberOfGuests { get; set; }
        }
    }
}
