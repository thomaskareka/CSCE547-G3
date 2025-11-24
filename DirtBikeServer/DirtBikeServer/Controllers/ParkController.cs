using DirtBikeServer.Data;
using DirtBikeServer.Exceptions;
using DirtBikeServer.Interfaces;
using DirtBikeServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;

namespace DirtBikeServer.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class ParkController: ControllerBase {
        private readonly IParkService _service;
        private readonly ILogger<ParkController> _logger;
        public ParkController(IParkService service, ILogger<ParkController> logger) {
            _service = service;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> AddPark([FromBody] ParkDTOs.CreateParkDTO dto) {
            try {
                var success = await _service.AddPark(dto);
                if (!success) {
                    return Problem("");
                }
                return Ok();
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error creating Park {dto}");
                return Problem(e.Message);
            }

        }

        [HttpDelete("{parkId:Guid}")]
        public async Task<IActionResult> RemovePark([FromRoute] Guid parkId) {
             try {
                var success = await _service.RemovePark(parkId);
                if (!success) {
                    _logger.LogError($"Error removing Park {parkId}");
                    return Problem("Error removing park.");
                }
                return Ok();
            } catch (ParkNotFoundException e) {
                _logger.LogInformation(e.Message);
                return NotFound(e.Message);
            } catch (ArgumentException e) {
                return BadRequest(e.Message);
            } catch (Exception e) {
                _logger.LogError(e, $"Error removing Park {parkId}");
                return Problem("Error removing park.");
            }
        }

        [HttpGet("{parkId:Guid}")]
        public async Task<IActionResult> GetPark([FromRoute] Guid parkId) {
            try {
                var parkResult = await _service.GetPark(parkId);
                if (parkResult != null) {
                    return Ok(parkResult);
                }
                return NotFound();
            }
            catch (ParkNotFoundException e) {
                _logger.LogInformation(e.Message);
                return NotFound(e.Message);
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error getting Park {parkId}");
                return Problem("Error getting park.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetParks() {
            try {
                var parkResults = await _service.GetParks();
                return Ok(parkResults);
            } catch (Exception e) {
                _logger.LogError(e, $"Error getting all parks.");
                return Problem("Error getting all parks.");
            }
        }

        [HttpPost("/guests")]
        public async Task<IActionResult> AddGuestLimitToPark([FromBody] ParkDTOs.GuestDTO dto) {
            try {
                var success = await _service.AddGuestLimitToPark(dto);
                if (success) {
                    return Ok();
                }
                return Problem();
            } catch (ArgumentException e) {
                return BadRequest(e.Message);
            } catch (Exception e) {
                _logger.LogError(e, $"Error modifying guest limit: {dto}.");
                return Problem("Error modifying guest limit.");
            }
        }
        
        [HttpDelete("/guests")]
        public async Task<IActionResult> RemoveGuestsFromPark([FromBody] ParkDTOs.GuestDTO dto) {
            try {
                var success = await _service.RemoveGuestsFromPark(dto);
                if (success) {
                    return Ok();
                }
                return Problem();
            }
            catch (ArgumentException e) {
                return BadRequest(e.Message);
            }
            catch (Exception e) {
                _logger.LogError(e, $"Error removing guests from park: {dto}.");
                return Problem("Error removing guests from park.");
            }
        }

        [HttpPut("edit/")]
        public async Task<IActionResult> EditPark([FromBody] ParkDTOs.EditParkDTO dto) {
            try {
                var success = await _service.EditPark(dto);
                if (success) {
                    return Ok();
                }
                return Problem();
            } catch (ParkNotFoundException e) {
                return NotFound(e.Message);
            } catch (ArgumentException e) {
                return BadRequest(e.Message);
            } catch (Exception e) {
                _logger.LogError(e, $"Error modifying park {dto}.");
                return Problem("Error getting all parks.");
            }
        }
    }
}
