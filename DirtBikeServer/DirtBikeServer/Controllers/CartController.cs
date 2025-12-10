using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirtBikeServer.Controllers {

    // Marks the class as a Web API controller and enables automatic model validation
    [ApiController]

    // Base route: /api/cart
    [Route("api/[controller]")]
    public class CartController : ControllerBase {

        private readonly ICartService _service;                // Business logic layer
        private readonly ILogger<CartController> _logger;      // Logging for diagnostics

        // Dependencies are resolved using ASP.NET Core Dependency Injection
        public CartController(ICartService service, ILogger<CartController> logger) {
            _service = service;
            _logger = logger;
        }

        // POST /add_booking
        // Adds a booking to a user cart (validates guest limits, park availability, etc.)
        [HttpPost("/add_booking")]
        public async Task<IActionResult> AddBookingToCart([FromBody] CartDTOs.CreateCartBookingDTO dto) {
            try {
                var success = await _service.AddBookingToCart(dto);

                // Service layer returns true if booking was successfully added
                if (success)
                    return Ok();

                // Bad request if validation failed or the DTO was invalid
                return BadRequest(dto);
            }
            catch (ParkFullException e) {
                // Park has reached its guest limit
                return Conflict(e.Message);
            }
            catch (ArgumentException e) {
                // Handle invalid input or DTO values
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                // Unexpected failure
                _logger.LogError(e, $"Error occurred while adding booking to cart {dto.CartId}");
                return Problem("Could not create booking.");
            }
        }

        // PUT /remove_booking
        // Removes a booking from the cart and returns the updated cart object
        [HttpPut("/remove_booking")]
        public async Task<IActionResult> RemoveBookingFromCart([FromBody] CartDTOs.RemoveBookingDTO dto) {
            try {
                var newCart = await _service.RemoveBookingFromCart(dto);

                // If successful, return updated cart summary
                if (newCart != null)
                    return Ok(newCart);

                // Meaning input or booking data was invalid
                return BadRequest(dto);
            }
            catch (ArgumentException e) {
                // Invalid data such as malformed booking ID
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e) {
                // Booking or cart not found
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e) {
                // Covers removing too many guests or internal state issues
                _logger.LogError(e, $"Could not remove booking from cart. {dto}");
                return Problem(e.Message);
            }
            catch (Exception e) {
                // Default fallback
                _logger.LogError(e, $"Error occurred in removing booking {dto}");
                return Problem("Could not remove booking.");
            }
        }

        // GET /api/cart/{cartId}
        // Retrieves a specific cart by its GUID identifier
        [HttpGet("{cartId:Guid}")]
        public async Task<IActionResult> GetCart([FromRoute] Guid cartId) {
            try {
                var responseDto = await _service.GetCart(cartId);

                if (responseDto != null)
                    return Ok(responseDto);

                // Cart doesn't exist
                return BadRequest(cartId);
            }
            catch (InvalidOperationException e) {
                // Occurs when a cart is in an invalid state
                _logger.LogError(e, $"Obtaining cart {cartId} failed.");
                return Problem(e.Message);
            }
            catch (ArgumentException e) {
                // Invalid GUID or improper input
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, "Error occurred in obtaining cart.");
                return Problem(e.Message);
            }
        }

        // GET /api/cart
        // Retrieves the active session cart or creates a new one if none exists
        [HttpGet]
        public async Task<IActionResult> GetCart() {
            try {
                var responseDto = await _service.GetCart();

                if (responseDto != null)
                    return Ok(responseDto);

                // If null, something failed at the service layer
                return Problem();
            }
            catch (InvalidOperationException e) {
                _logger.LogError(e, "Creating cart failed.");
                return Problem(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, "Error occurred in creating cart.");
                return Problem(e.Message);
            }
        }

        // POST /payment
        // Processes the payment for a cart using card details supplied in the DTO
        [HttpPost("/payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] CartDTOs.ProcessPaymentDTO dto) {
            try {
                var success = await _service.ProcessPayment(dto);

                if (success)
                    return Ok();

                return Problem(); // Payment validation failed
            }
            catch (Exception e) {
                // Generic failure: invalid card, DB error, etc.
                return Problem(e.Message);
            }
        }
    }
}
