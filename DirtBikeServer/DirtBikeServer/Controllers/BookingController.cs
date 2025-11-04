using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;
namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController: ControllerBase {
        private readonly IBookingService _service;
        public BookingController(IBookingService service) => _service = service;

        [HttpGet("{bookingId:Guid}")]
        public async Task<IActionResult> GetBooking([FromRoute] Guid bookingId)
            => Ok(await _service.GetBooking(bookingId));

        [HttpGet]
        public async Task<IActionResult> GetBookings()
            => Ok(await _service.GetBookings());

        [HttpDelete("{bookingId:Guid}")]
        public async Task<IActionResult> RemoveBooking([FromRoute] Guid bookingId)
          => Ok(await _service.RemoveBooking(bookingId));

        [HttpPost("{parkId:Guid}")]
        public async Task<IActionResult> CreateBooking([FromRoute] Guid parkId, [FromBody] BookingDTOs.CreateBookingDTO dto)
            => Ok(await _service.CreateBooking(parkId, dto.Adults, dto.Children, dto.CartId));
    }
}
