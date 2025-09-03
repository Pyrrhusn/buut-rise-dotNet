using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Boats;

namespace Rise.Server.Controllers.Boats
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Administrator")]
    public class BatteryController(IBatteryService batteryService, ILogger<BatteryController> logger) : ControllerBase
    {

        private readonly ILogger<BatteryController> _logger = logger;
        private readonly IBatteryService _batteryService = batteryService;

        /// <summary>
        /// Gets the information of a battery
        /// </summary>
        /// <param name="id">battery id</param>
        /// <returns>The battery information</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatteryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBattery(
            [Range(0, int.MaxValue, ErrorMessage = "Battery id must be positive")]
            int id)
        {
            _logger.LogInformation("GET battery: {id}", id);
            BatteryDto battery = await _batteryService.GetBattery(id);
            _logger.LogInformation("GET done fetching battery: {id}", id);
            return Ok(battery);
        }

        /// <summary>
        /// Update the information of a battery
        /// </summary>
        /// <param name="id">battery id</param>
        /// <param name="newBattery">information to update the battery with</param>
        /// <returns>The updated battery</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(BatteryDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> PutBattery(
            [Range(0, int.MaxValue, ErrorMessage = "Battery id must be positive")]
            int id,
            [FromBody]
            BatteryUpdateDto newBattery)
        {
            _logger.LogInformation("PUT battery: {id}", id);
            _logger.LogDebug("PUT battery: {id} with information {newBattery}", id, newBattery);
            BatteryDto battery = await _batteryService.UpdateBattery(id, newBattery);
            _logger.LogInformation("PUT done updating battery: {id}", id);
            _logger.LogDebug("PUT done updating battery: {id}. New information {battery}", id, battery);
            return Ok(battery);
        }



        /// <summary>
        /// Get all batteries for a boat
        /// </summary>
        /// <param name="boatId">The boat id for which you want battries</param>
        /// <returns>All the battries for a specfic boat</returns>
        [HttpGet("boat/{boatId}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<BatteryDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetBatteriesByBoat(
        [Range(0, int.MaxValue, ErrorMessage = "Boat id must be positive")]
        int boatId)
        {
            _logger.LogInformation("GET batteries for boat: {boatId}", boatId);
            IEnumerable<BatteryDto> batteries = await _batteryService.GetBatteriesByBoat(boatId);
            _logger.LogInformation("GET done fetching batteries for boat: {boatId}", boatId);
            return Ok(batteries);
        }


    }
}
