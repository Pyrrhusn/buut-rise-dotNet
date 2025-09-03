using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Domain.Exceptions;
using Rise.Shared;
using Rise.Shared.Boats;

namespace Rise.Server.Controllers.Boats
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class BoatController(IBoatService boatService, ILogger<BoatController> logger) : ControllerBase
    {
        private readonly ILogger<BoatController> _logger = logger;
        private readonly IBoatService _boatService = boatService;
        /// <summary>
        /// Gets the count of active boats
        /// </summary>
        /// <returns>The number of non-deleted boats</returns>
        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActiveBoatsCount()
        {
            _logger.LogInformation("GET boats count");
            var count = await _boatService.GetActiveBoatsCountAsync();
            _logger.LogInformation("GET done fetching boats count: {count}", count);
            return Ok(count);
        }

        /// <summary>
        /// Haalt alle boten op.
        /// </summary>
        /// <returns>Een lijst van boten.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllBoats()
        {
            try
            {
                var boats = await _boatService.GetAllBoatsAsync();
                return Ok(boats);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Er is een fout opgetreden: {ex.Message}");
            }
        }

        /// <summary>
        /// Wijzigt de beschikbaarheid van een boot.
        /// </summary>
        /// <param name="boatId">Het ID van de boot.</param>
        /// <param name="isAvailable">De nieuwe beschikbaarheidsstatus.</param>
        /// <returns>Geen inhoud als succesvol.</returns>
        [HttpPatch("{boatId}/availability")]
        public async Task<IActionResult> UpdateBoatAvailabilityAsync(
            [FromRoute] int boatId,
            [FromBody] bool isAvailable)
        {
            try
            {
                await _boatService.UpdateBoatAvailabilityAsync(boatId, isAvailable);
                return NoContent();
            }
            catch (EntityNotFoundException ex)
            {
                _logger.LogWarning("Boot niet gevonden: {Message}", ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Er is een fout opgetreden bij het bijwerken van de beschikbaarheid");
                return StatusCode(500, $"Er is een fout opgetreden: {ex.Message}");
            }

        }

        /// <summary>
        /// Creates a new boat.
        /// </summary>
        /// <returns>The id of the created boat.</returns>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateBoat([FromBody] CreateBoatDto createBoatDto)
        {
            var boatId = await _boatService.CreateBoatAsync(createBoatDto);
            return Ok(boatId);

        }
    }
}
