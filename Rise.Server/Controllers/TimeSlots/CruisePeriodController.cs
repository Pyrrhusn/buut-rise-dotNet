using Microsoft.AspNetCore.Mvc;
using Rise.Shared.TimeSlots;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Rise.Shared.Users;

namespace Rise.Server.Controllers.TimeSlots
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = $"{nameof(UserRole.Guest)},{nameof(UserRole.Member)}")]
    public class CruisePeriodController(ICruisePeriodService cruisePeriodService, ILogger<CruisePeriodController> logger) : ControllerBase
    {
        private readonly ILogger _logger = logger;
        private readonly ICruisePeriodService _cruisePeriodService = cruisePeriodService;

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CruisePeriodDetailedDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCruisePeriod(
            [Range(0, int.MaxValue, ErrorMessage = "Cruise period id must be positive")]
            int id
            )
        {
            _logger.LogInformation("GET Cruise Period {id}", [id]);
            CruisePeriodDetailedDto cruisePeriod = await _cruisePeriodService.GetCruisePeriod(id);
            _logger.LogInformation("GET done fetching Cruise Period {id}", [id]);
            return Ok(cruisePeriod);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CruisePeriodDetailedDto>))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCruisePeriods([FromQuery] bool getFuturePeriods = true)
        {
            _logger.LogInformation("GET Cruise Periods (getFuturePeriods: {getFuturePeriods})", getFuturePeriods);
            var cruisePeriods = await _cruisePeriodService.GetCruisePeriods(getFuturePeriods);
            return Ok(cruisePeriods);
        }

    }
}

