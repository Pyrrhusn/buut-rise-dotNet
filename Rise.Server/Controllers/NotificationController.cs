using Ardalis.GuardClauses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Notifications;

namespace Rise.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {

        private readonly ILogger<NotificationController> _logger;
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService, ILogger<NotificationController> logger)
        {
            _logger = logger;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Retrieves a list of notifications for the current user.
        /// </summary>
        /// <param name="limit">An optional limit to the number of notifications returned. Must be a non-negative integer.</param>
        /// <returns>A list of notifications for the current user.</returns>
        /// <response code="200">Returns the list of notifications.</response>
        /// <response code="400">The limit parameter contains an invalid value (e.g., negative).</response>
        [HttpGet("me")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<NotificationDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCurrentUserNotifications([FromQuery] int? limit)
        {
            if (limit.HasValue && limit.Value < 0)
            {
                _logger.LogWarning("Invalid limit: {limit} is negative.", [limit.Value]);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "Limit", [$"The limit cannot contain negative values ({limit})."] }
                }));
            }
            try
            {
                var notifications = await _notificationService.GetUserNotifications(limit);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching notifications for the current user.");
                return Problem("An error occurred while fetching notifications for the current user.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Retrieves the count of unread notifications for the current user.
        /// </summary>
        /// <returns>The number of unread notifications.</returns>
        /// <response code="200">Returns the count of unread notifications.</response>
        /// <response code="500">An internal server error occurred while processing the request.</response>
        [HttpGet("me/unread/count")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUnreadNotificationCount()
        {
            try
            {
                var count = await _notificationService.GetUnreadNotificationCount();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while fetching the unread notification count for the current user.");
                return Problem("An error occurred while fetching the unread notification count for the current user.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>
        /// Marks a specific notification as read.
        /// </summary>
        /// <param name="id">The ID of the notification to mark as read. Must be a positive integer.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">The notification was successfully marked as read.</response>
        /// <response code="404">The notification with the specified ID was not found.</response>
        /// <response code="500">An internal server error occurred while processing the request.</response>
        [HttpPatch("read/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> MarkNotificationAsRead(int id)
        {
            if (id <= 0)
            {
                _logger.LogWarning("Invalid notification ID: {id} is not a valid ID.", id);
                return ValidationProblem(new ValidationProblemDetails(new Dictionary<string, string[]>
                {
                    { "Id", [$"The notification ID must be a positive integer ({id})."] }
                }));
            }
            try
            {
                await _notificationService.MarkNotificationAsRead(id);
                return NoContent();
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Notification {id} not found.", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while marking notification {id} as read.", id);
                return Problem("An error occurred while marking the notification as read.", statusCode: StatusCodes.Status500InternalServerError);
            }
        }

    }
}
