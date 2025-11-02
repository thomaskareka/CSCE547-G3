using DirtBikeServer.Data;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class CartController: ControllerBase {
        private readonly ICartService _service;
        public CartController(ICartService service) => _service = service;

        [HttpPost("/add_booking")]
        public async Task<IActionResult> AddBookingToCart([FromBody] CartDTOs.CreateBookingDTO dto)
          => Ok(await _service.AddBookingToCart(dto.CartId, dto.ParkId, dto.BookingInfo));

        [HttpPut("/remove_booking")]
        public async Task<IActionResult> RemoveBookingFromCart([FromBody] CartDTOs.RemoveBookingDTO dto)
            => Ok(await _service.RemoveBookingFromCart(dto.CartId, dto.BookingId));

        [HttpGet("{cartId:Guid}")]
        public async Task<IActionResult> GetCart([FromRoute] Guid cartId)
            => Ok(await _service.GetCart(cartId));

        [HttpGet]
        public async Task<IActionResult> GetCart()
            => Ok(await _service.GetCart());

        [HttpPost("/payment")]
        public async Task<IActionResult> ProcessPayment([FromBody] CartDTOs.ProcessPaymentDTO dto)
            => Ok(await _service.ProcessPayment(dto.CartId, dto.CardNumber, dto.ExpirationDate, dto.CardHolderName, dto.Cvc));
    }
}
