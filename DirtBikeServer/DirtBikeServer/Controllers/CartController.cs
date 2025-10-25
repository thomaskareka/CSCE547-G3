using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CartController: ControllerBase {
        private readonly ICartService _service;
        public CartController(ICartService service) => _service = service;

        [HttpPost("{cartId:int}")]
        public async Task<IActionResult> GetBooking([FromQuery] int cartId, int parkId, Booking bookingInfo)
          => Ok(await _service.AddBookingToCart(cartId, parkId, bookingInfo));

        [HttpPut("{cartId:int}")]
        public async Task<IActionResult> RemoveBookingFromCart([FromQuery] int cartId, int bookingId)
            => Ok(await _service.RemoveBookingFromCart(cartId, bookingId));

        [HttpGet("{cartId:int}")]
        public async Task<IActionResult> GetCart([FromQuery] int cartId)
            => Ok(await _service.GetCart(cartId));

        [HttpGet]
        public async Task<IActionResult> GetCart()
            => Ok(await _service.GetCart());

        [HttpPost("{cartId:int}/payment")]
        public async Task<IActionResult> ProcessPayment([FromQuery] int cartId, string cardNumber, DateTime exp, string cardHolderName, int cvc)
            => Ok(await _service.ProcessPayment(cartId, cardNumber, exp, cardHolderName, cvc));
    }
}
