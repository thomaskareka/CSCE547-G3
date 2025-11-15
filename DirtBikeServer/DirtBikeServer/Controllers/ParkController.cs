using DirtBikeServer.Data;
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
        public async Task<IActionResult> AddPark([FromBody] ParkDTOs.CreateParkDTO dto)
            => Ok(await _service.AddPark(dto));

        [HttpDelete("{parkId:Guid}")]
        public async Task<IActionResult> RemovePark([FromRoute] Guid parkId)
            => Ok(await _service.RemovePark(parkId));

        [HttpGet("{parkId:Guid}")]
        public async Task<IActionResult> GetPark([FromRoute] Guid parkId)
            => Ok(await _service.GetPark(parkId));

        [HttpGet]
        public async Task<IActionResult> GetParks()
            => Ok(await _service.GetParks());

        [HttpPost("/guests")]
        public async Task<IActionResult> AddGuestLimitToPark([FromBody] ParkDTOs.GuestDTO dto)
            => Ok(await _service.AddGuestLimitToPark(dto));
        
        [HttpDelete("/guests")]
        public async Task<IActionResult> RemoveGuestsFromPark([FromBody] ParkDTOs.GuestDTO dto)
            => Ok(await _service.RemoveGuestsFromPark(dto));

        [HttpPut("edit/")]
        public async Task<IActionResult> EditPark([FromBody] ParkDTOs.EditParkDTO dto)
            => Ok(await _service.EditPark(dto));


    }
}
