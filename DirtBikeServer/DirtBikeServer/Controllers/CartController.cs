using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CartController: ControllerBase {
        private readonly ICartService _service;
        public CartController(ICartService service) => _service = service;

        [HttpPost("{cartId:Guid}")]
        public async Task<IActionResult> GetBooking([FromQuery] Guid cartId, Guid parkId, Booking bookingInfo)
          => Ok(await _service.AddBookingToCart(cartId, parkId, bookingInfo));

        [HttpPut("{cartId:Guid}")]
        public async Task<IActionResult> RemoveBookingFromCart([FromQuery] Guid cartId, Guid bookingId)
            => Ok(await _service.RemoveBookingFromCart(cartId, bookingId));

        [HttpGet("{cartId:Guid}")]
        public async Task<IActionResult> GetCart([FromQuery] Guid cartId)
            => Ok(await _service.GetCart(cartId));

        [HttpGet]
        public async Task<IActionResult> GetCart()
            => Ok(await _service.GetCart());

        [HttpPost("{cartId:Guid}/payment")]
        public async Task<IActionResult> ProcessPayment([FromQuery] Guid cartId, string cardNumber, DateTime exp, string cardHolderName, int cvc)
            => Ok(await _service.ProcessPayment(cartId, cardNumber, exp, cardHolderName, cvc));
    }
}
