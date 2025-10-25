using DirtBikeServer.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController: ControllerBase {
        private readonly IBookingService _service;
        public BookingController(IBookingService service) => _service = service;

        [HttpGet("{bookingId:int}")]
        public async Task<IActionResult> GetBooking([FromQuery] int bookingId)
            => Ok(await _service.GetBooking(bookingId));

        [HttpGet]
        public async Task<IActionResult> GetBookings()
            => Ok(await _service.GetBookings());

        [HttpDelete("{bookingId:int}")]
        public async Task<IActionResult> RemoveBooking([FromQuery] int bookingid)
          => Ok(await _service.RemoveBooking(bookingid));

        [HttpPost("{bookingId:int}")]
        public async Task<IActionResult> CreateBooking([FromQuery] int bookingid)
            => Ok(await _service.CreateBooking(bookingid));
    }
}
