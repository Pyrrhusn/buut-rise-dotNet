using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Server.Common.Filters;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Rise.Domain.Exceptions;
using Rise.Shared.Users;
namespace Rise.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]

    public class ReservationController(ILogger<ReservationController> logger, IReservationService reservationService) : ControllerBase
    {
        private readonly ILogger<ReservationController> _logger = logger;
        private readonly IReservationService _reservationService = reservationService;

        /// <summary>
        /// Gets all reservations for a user.
        /// </summary>
        /// <remarks>
        /// <para>To get the first page, set cursor and isNextPage to null. For ther other pages, use the following logic:</para> 
        /// <para>How it works, set the cursor to an Id you get from a request with the following valid parameters (not null):
        /// <br/>
        /// <para>NextId and isNextPage equals true: get the next page </para>
        /// <br/>
        /// <para>PreviousId and isNextPage equals false: get the previous page</para></para>
        /// </remarks>
        /// 
        /// <param name="cursor">The Id of a entity to fetch relative to, i.e. cursor can be the Id of NextId or PreviousId (see above).</param>
        /// <param name="isNextPage">If avaiable, true to get next page or false: to get last page.</param>   
        /// <param name="getPast">Get all reservations in the past.</param>
        /// <param name="pageSize">Number of items to get in a page.</param>
        /// <returns>List of reservations</returns>
        [HttpGet("me")]
        [Authorize(Roles = nameof(UserRole.Member))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemsPageDto<ReservationDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetCurrentUserReservations(
            [FromQuery] int? cursor,
            [FromQuery] bool? isNextPage,
            [FromQuery] bool getPast = false,
            [FromQuery] int pageSize = 5)
        {
            if (cursor < 0)
            {
                _logger.LogWarning("Invalid cursor id: {cursor} is negative.", [cursor]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "Cursor", [$"The cursor cannot contain negative ids ({cursor})."] }
                }));
            }

            if (pageSize < 1)
            {
                _logger.LogWarning("Invalid page size: {pageSize} is negative.", [pageSize]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "PageSize", [$"Page size must be a whole positive number; {pageSize}."] }
                }));
            }

            if (cursor is not null && isNextPage is null)
            {
                _logger.LogWarning("Invalid paging direction {isNextPage}, but cursor was correct.", [isNextPage]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "Cursor", [$"Correctly given cursor; {cursor}."] },
                    {"IsNextPage", [$"Paging direction is null, specify forward (true) or backwards (false) direction."]}
                }));
            }

            var reservations = await _reservationService.GetUserReservations(cursor, isNextPage, getPast, pageSize);
            return Ok(reservations);
        }
        /// <summary>
        /// Creates a reservation for a specific timeslot for the current user
        /// </summary>
        /// <param name="reservationDto">The details of the reservation to create</param>
        /// <returns>The deatils of the created reservation</returns>
        [HttpPost]
        [Authorize(Roles = nameof(UserRole.Member))]
        [NoQueryParameters]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateReservation([FromBody] CreateReservationDto reservationDto)
        {
            var reservationId = await _reservationService.CreateReservation(reservationDto);
            return CreatedAtAction(nameof(CreateReservation), reservationId);
        }



        /// <summary>
        /// Retrieves the details of a specific reservation by its ID.
        /// </summary>
        /// <param name="id">The ID of the reservation to retrieve.</param>
        /// <returns>The details of the reservation.</returns>
        /// <response code="200">Returns the reservation details if found.</response>
        /// <response code="400">If the ID is invalid or missing.</response>
        /// <response code="404">If no reservation with the specified ID is found.</response>
        /// <response code="500">If an error occurs while retrieving the reservation details.</response>
        [HttpGet("{id}")]
        [Authorize(Roles = nameof(UserRole.Member))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReservationDetailsDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetReservationDetails(int id)
        {
            try
            {
                var reservationDetails = await _reservationService.GetReservationDetailsAsync(id);
                return Ok(reservationDetails);
            }
            catch (EntityNotFoundException)
            {
                _logger.LogWarning("Reservation with id {id} not found.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving reservation details for id {id}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving the reservation details.");
            }
        }

        [HttpPatch("cancel/{id}")]
        [Authorize(Roles = $"{nameof(UserRole.Administrator)},{nameof(UserRole.Member)}")]

        public async Task<IActionResult> CancelReservation(int id)
        {
            try
            {
                bool isAdmin = User.IsInRole("Administrator");
                await _reservationService.CancelReservationAsync(id);
                return NoContent();
            }
            catch (EntityNotFoundException)
            {
                _logger.LogWarning("Reservation with id {id} not found.", id);
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets all reservations in the system with pagination support for admins.
        /// </summary>
        /// <remarks>
        /// This endpoint is accessible only by admins and supports pagination. 
        /// The admin can fetch reservations for all users in the system.
        /// </remarks>
        /// <param name="cursor">The ID of a reservation to fetch relative to, for pagination.</param>
        /// <param name="isNextPage">If available, true to get the next page or false to get the previous page.</param>
        /// <param name="pageSize">Number of items to get in a page (default is 10).</param>
        /// <param name="showPastReservations"></param>
        /// <returns>Paginated list of all reservations in the system.</returns>
        [HttpGet("all")]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ItemsPageDto<ReservationDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllReservations(
            [FromQuery] int? cursor,
            [FromQuery] bool? isNextPage,
            [FromQuery] int pageSize = 10,
            [FromQuery] bool showPastReservations = false)

        {
            try
            {
                var reservations = await _reservationService.GetAllReservations(cursor, isNextPage, pageSize, showPastReservations);
                return Ok(reservations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while fetching all reservations.");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while retrieving reservations.");
            }
        }




        /// <summary>
        /// Gets the count of reservations for a given date
        /// </summary>
        /// <param name="date">The date for which to get count of reservations</param>
        /// <returns>The count of reservations for the date</returns>
        [HttpGet("count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetReservationsCount([FromQuery] DateOnly date)
        {
            var count = await _reservationService.GetReservationsCountAsync(date);
            return Ok(count);
        }
    }
}

