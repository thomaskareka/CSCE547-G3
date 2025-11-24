using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;
namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class BookingController : ControllerBase {
        private readonly IBookingService _service;
        private readonly ILogger<BookingController> _logger;
        public BookingController(IBookingService service, ILogger<BookingController> logger) {
            _service = service;
            _logger = logger;
        }

        [HttpGet("{bookingId:Guid}")]
        public async Task<IActionResult> GetBooking([FromRoute] Guid bookingId) {
            try {
                var bookingResponse = await _service.GetBooking(bookingId);
                if (bookingResponse == null) {
                    return NotFound();
                }
                return Ok(bookingResponse);
            }
            catch (BookingNotFoundException e) {
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error occured in getting booking {bookingId}");
                return Problem("Could not get booking.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetBookings() {
            try {
                var bookingList = await _service.GetBookings();
                if (bookingList == null) {
                    return NotFound();
                }
                return Ok(bookingList);
            }
            catch (Exception e) {
                _logger.LogError(e, "Error occured in getting all bookings.");
                return Problem("Could not get all bookings.");
            }
        }

        [HttpDelete("{bookingId:Guid}")]
        public async Task<IActionResult> RemoveBooking([FromRoute] Guid bookingId) {
            try {
                var success = await _service.RemoveBooking(bookingId);
                if (!success) {
                    return NotFound();
                }
                return Ok();
            }
            catch (BookingNotFoundException e) {
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Could not delete booking {bookingId}");
                return Problem($"Could not remove booking {bookingId}.");
            }
        }

        [HttpPost("{parkId:Guid}")]
        public async Task<IActionResult> CreateBooking([FromRoute] Guid parkId, [FromBody] BookingDTOs.CreateBookingDTO dto) {
            try {
                var success = await _service.CreateBooking(parkId, dto);
                if (!success) {
                    return BadRequest();
                }
                return Ok();
            }
            catch (BookingNotFoundException e) {
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"An error occured in creating a booking for {parkId}: {dto}");
                return Problem($"Could not create a booking for park {parkId}");
            }
        }
    }
}
