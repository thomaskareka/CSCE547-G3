using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ParkController: ControllerBase {
        private readonly IParkService _service;
        public ParkController(IParkService service) => _service = service;

        [HttpPost]
        public async Task<IActionResult> AddPark([FromQuery] Park park)
            => Ok(await _service.AddPark(park));

        [HttpDelete("{parkId:Guid}")]
        public async Task<IActionResult> RemovePark([FromQuery] Guid parkId)
            => Ok(await _service.RemovePark(parkId));

        [HttpGet("{parkId:Guid}")]
        public async Task<IActionResult> GetPark([FromQuery] Guid parkId)
            => Ok(await _service.GetPark(parkId));

        [HttpGet]
        public async Task<IActionResult> GetParks()
            => Ok(await _service.GetParks());

        [HttpPost("{parkId:Guid}/guests")]
        public async Task<IActionResult> AddGuestLimitToPark([FromQuery] Park park, int numberOfGuests)
            => Ok(await _service.AddGuestLimitToPark(park, numberOfGuests));
        
        [HttpDelete("{parkId:Guid}/guests")]
        public async Task<IActionResult> RemoveGuestsFromPark([FromQuery] Park park, int numberOfGuests)
            => Ok(await _service.RemoveGuestsFromPark(park, numberOfGuests));

        [HttpPut("{parkId:Guid}")]
        public async Task<IActionResult> EditPark([FromQuery] Park park, Park newPark)
            => Ok(await _service.EditPark(park, newPark));


    }
}
