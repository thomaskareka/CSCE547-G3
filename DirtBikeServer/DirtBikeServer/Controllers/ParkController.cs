using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;

namespace DirtBikeServer.Controllers {

    // Identifies this class as a Web API controller with automatic model validation.
    [ApiController]

    // Base route: /api/park
    [Route("api/[controller]")]
    public class ParkController : ControllerBase {

        private readonly IParkService _service;                 // Business logic layer
        private readonly ILogger<ParkController> _logger;       // Logger for diagnostics

        // Constructor uses Dependency Injection to provide service + logger
        public ParkController(IParkService service, ILogger<ParkController> logger) {
            _service = service;
            _logger = logger;
        }

        // POST /api/park
        // Creates a new park with initial details (name, location, prices)
        [HttpPost]
        public async Task<IActionResult> AddPark([FromBody] ParkDTOs.CreateParkDTO dto) {
            try {
                var success = await _service.AddPark(dto);

                // The service returns false when creation fails for known reasons
                if (!success) {
                    return Problem("");
                }

                return Ok(); // Park successfully created
            }
            catch (ArgumentException e) {
                // Invalid input data (missing fields, invalid price, etc.)
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                // Unexpected errors get logged and handled generically
                _logger.LogError(e, $"Error creating Park {dto}");
                return Problem(e.Message);
            }
        }

        // DELETE /api/park/{parkId}
        // Removes a park completely from the system
        [HttpDelete("{parkId:Guid}")]
        public async Task<IActionResult> RemovePark([FromRoute] Guid parkId) {
            try {
                var success = await _service.RemovePark(parkId);

                if (!success) {
                    _logger.LogError($"Error removing Park {parkId}");
                    return Problem("Error removing park.");
                }

                return Ok(); // Successful deletion
            }
            catch (ParkNotFoundException e) {
                // Attempted to delete a park that does not exist
                _logger.LogInformation(e.Message);
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                // Invalid GUID or other bad input
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error removing Park {parkId}");
                return Problem("Error removing park.");
            }
        }

        // GET /api/park/{parkId}
        // Retrieves a single park’s full details
        [HttpGet("{parkId:Guid}")]
        public async Task<IActionResult> GetPark([FromRoute] Guid parkId) {
            try {
                var parkResult = await _service.GetPark(parkId);

                // Return 200 + park object if found
                if (parkResult != null) {
                    return Ok(parkResult);
                }

                return NotFound();
            }
            catch (ParkNotFoundException e) {
                // Specific park not found
                _logger.LogInformation(e.Message);
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                // Invalid GUID or malformed request
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error getting Park {parkId}");
                return Problem("Error getting park.");
            }
        }

        // GET /api/park
        // Retrieves all parks in the system
        [HttpGet]
        public async Task<IActionResult> GetParks() {
            try {
                var parkResults = await _service.GetParks();
                return Ok(parkResults);
            }
            catch (Exception e) {
                _logger.LogError(e, "Error getting all parks.");
                return Problem("Error getting all parks.");
            }
        }

        // POST /guests
        // Adds or updates the guest limit for a specific park
        [HttpPost("/guests")]
        public async Task<IActionResult> AddGuestLimitToPark([FromBody] ParkDTOs.GuestDTO dto) {
            try {
                var success = await _service.AddGuestLimitToPark(dto);

                if (success) {
                    return Ok();
                }

                return Problem();
            }
            catch (ArgumentException e) {
                // Invalid parkId or negative guest count
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error modifying guest limit: {dto}.");
                return Problem("Error modifying guest limit.");
            }
        }

        // DELETE /guests
        // Removes guests from a park based on bookings (used when cart restores inventory)
        [HttpDelete("/guests")]
        public async Task<IActionResult> RemoveGuestsFromPark([FromBody] ParkDTOs.RemoveGuestDTO dto) {
            try {
                var success = await _service.RemoveGuestsFromPark(dto);

                if (success) {
                    return Ok();
                }

                return Problem();
            }
            catch (ArgumentException e) {
                // Invalid request such as removing more guests than allowed
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error removing guests from park: {dto}.");
                return Problem("Error removing guests from park.");
            }
        }

        // PUT /api/park/edit
        // Updates park details (prices, guest limits, name, etc.)
        [HttpPut("edit/")]
        public async Task<IActionResult> EditPark([FromBody] ParkDTOs.EditParkDTO dto) {
            try {
                var success = await _service.EditPark(dto);

                if (success) {
                    return Ok();
                }

                return Problem();
            }
            catch (ParkNotFoundException e) {
                // Cannot modify a park that doesn't exist
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                // Invalid price, limit, or other DTO values
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                // Unexpected failure
                _logger.LogError(e, $"Error modifying park {dto}.");
                return Problem("Error getting all parks.");
            }
        }
    }
}
