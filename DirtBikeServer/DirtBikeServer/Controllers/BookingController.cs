using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirtBikeServer.Controllers {

    // Marks this as a Web API controller and enables automatic model validation
    [ApiController]

    // Sets the base route to: /api/booking
    [Route("api/[controller]")]
    public class BookingController : ControllerBase {

        private readonly IBookingService _service;              // Business logic layer dependency
        private readonly ILogger<BookingController> _logger;    // Logger for error tracking

        // Controller receives service + logger via dependency injection
        public BookingController(IBookingService service, ILogger<BookingController> logger) {
            _service = service;
            _logger = logger;
        }

        // GET /api/booking/{bookingId}
        // Fetches a single booking by unique GUID ID
        [HttpGet("{bookingId:Guid}")]
        public async Task<IActionResult> GetBooking([FromRoute] Guid bookingId) {
            try {
                var bookingResponse = await _service.GetBooking(bookingId);

                // Service returns null if the booking doesn't exist
                if (bookingResponse == null) {
                    return NotFound();
                }

                return Ok(bookingResponse);  // Return booking with 200 OK
            }
            catch (BookingNotFoundException e) {
                // Custom domain exception for "not found"
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                // Handles invalid GUID or bad input
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                // Generic exception handler logs unexpected failures
                _logger.LogError(e, $"Error occured in getting booking {bookingId}");
                return Problem("Could not get booking.");
            }
        }

        // GET /api/booking
        // Returns a list of all bookings in the system
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

        // DELETE /api/booking/{bookingId}
        // Removes a booking by ID
        [HttpDelete("{bookingId:Guid}")]
        public async Task<IActionResult> RemoveBooking([FromRoute] Guid bookingId) {
            try {
                var success = await _service.RemoveBooking(bookingId);

                // Service returns false if the booking was not found
                if (!success) {
                    return NotFound();
                }

                return Ok();  // Successful deletion
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

        // POST /api/booking/{parkId}
        // Creates a new booking for a specified park using DTO body data
        [HttpPost("{parkId:Guid}")]
        public async Task<IActionResult> CreateBooking([FromRoute] Guid parkId, [FromBody] BookingDTOs.CreateBookingDTO dto) {
            try {
                var success = await _service.CreateBooking(parkId, dto);

                // Booking may fail due to validation or park not found
                if (!success) {
                    return BadRequest();
                }

                return Ok(); // Successfully created
            }
            catch (BookingNotFoundException e) {
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                // Logs unexpected error along with attempted DTO content
                _logger.LogError(e, $"An error occured in creating a booking for {parkId}: {dto}");
                return Problem($"Could not create a booking for park {parkId}");
            }
        }
    }
}
