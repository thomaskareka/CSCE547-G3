using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CartController: ControllerBase {
        private readonly ICartService _service;
        private readonly ILogger<CartController> _logger;
        public CartController(ICartService service, ILogger<CartController> logger) {
            _service = service;
            _logger = logger;
        }

        [HttpPost("/add_booking")]
        public async Task<IActionResult> AddBookingToCart([FromBody] CartDTOs.CreateCartBookingDTO dto) {
            try {
                var success = await _service.AddBookingToCart(dto);
                if (success)
                    return Ok();
                return BadRequest(dto);
            }
            catch (ParkFullException e) {
                return Conflict(e.Message);
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error occured in adding booking to cart {dto.CartId}");
                return Problem("Could not create booking.");
            }
        }

        [HttpPut("/remove_booking")]
        public async Task<IActionResult> RemoveBookingFromCart([FromBody] CartDTOs.RemoveBookingDTO dto) {
            try {
                var newCart = await _service.RemoveBookingFromCart(dto);
                if (newCart != null)
                    return Ok(newCart);
                return BadRequest(dto);
            }
            catch(ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (KeyNotFoundException e) {
                return NotFound(e.Message);
            }
            catch (InvalidOperationException e) {
                _logger.LogError(e, $"Could not remove booking from cart. {dto}");
                return Problem(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error occured in removing booking {dto}");
                return Problem("Could not remove booking.");
            }
        }

        [HttpGet("{cartId:Guid}")]
        public async Task<IActionResult> GetCart([FromRoute] Guid cartId) {
            try {
                var responseDto = await _service.GetCart(cartId);
                if (responseDto != null) 
                    return Ok(responseDto);
                return BadRequest(cartId);
            }
            catch (InvalidOperationException e) {
                _logger.LogError(e, $"Obtaining cart {cartId} failed.");
                return Problem(e.Message);
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, "Error occurred in obtaining cart.");
                return Problem(e.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCart() {
            try {
                var responseDto = await _service.GetCart();
                if (responseDto != null)
                    return Ok(responseDto);
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

        [HttpPost("/payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] CartDTOs.ProcessPaymentDTO dto) {
            try {
                var success = await _service.ProcessPayment(dto);
                if (success)
                    return Ok();
                return Problem();
            } catch (Exception e) {
                return Problem(e.Message);
            }
        }
    }
}
