using DirtBikeServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController: ControllerBase {
        private readonly IBookingService _service;
        public BookingController(IBookingService service) => _service = service;

        [HttpGet("{bookingId:Guid}")]
        public async Task<IActionResult> GetBooking([FromQuery] Guid bookingId)
            => Ok(await _service.GetBooking(bookingId));

        [HttpGet]
        public async Task<IActionResult> GetBookings()
            => Ok(await _service.GetBookings());

        [HttpDelete("{bookingId:Guid}")]
        public async Task<IActionResult> RemoveBooking([FromQuery] Guid bookingid)
          => Ok(await _service.RemoveBooking(bookingid));

        [HttpPost("{bookingId:Guid}")]
        public async Task<IActionResult> CreateBooking([FromQuery] Guid bookingid)
            => Ok(await _service.CreateBooking(bookingid));
    }
}
