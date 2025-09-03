using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Users;

namespace Rise.Server.Controllers.Users
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(ILogger<UserController> logger, IUserAdminService userAdminService, IUserService userService) : ControllerBase
    {
        private readonly ILogger<UserController> _logger = logger;
        private readonly IUserAdminService _userAdminService = userAdminService;
        private readonly IUserService _userService = userService;

        /// <summary>
        /// Get users by role
        /// </summary>
        /// <param name="role">Role to filter users by (Administrator, Member, Guest).</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <returns>List of users matching the specified role</returns>
        [HttpGet]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<UserDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUsersByRole([FromQuery] UserRole role, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            _logger.LogInformation("GET api/User?role={Role}&page={Page}&pageSize={PageSize}", role, page, pageSize);

            try
            {
                var users = await _userAdminService.GetUsersByRole(role, page, pageSize);
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users for role: {Role}", role);
                return BadRequest("Error retrieving users");
            }
        }

        /// <summary>
        /// Gets the details of a user
        /// </summary>
        /// <returns>The details of a user</returns>
        [HttpGet("{userId}")]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDetailDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserDetails(int userId)
        {
            _logger.LogInformation("GET api/User/{userId}", userId);
            var details = await _userAdminService.GetUserDetails(userId);
            return Ok(details);
        }

        /// <summary>
        /// Gets the profile information of the logged in user
        /// </summary>
        /// <returns>The profile the logged in user</returns>
        [HttpGet("profile")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserProfileDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetUserProfile()
        {
            _logger.LogInformation("GET api/User/profile");
            var userProfile = await _userService.GetUserProfile();
            return Ok(userProfile);
        }

        /// <summary>
        /// Adds memeber role to a user
        /// </summary>
        /// <param name="request">Dto with id of the user to add member role to</param>
        /// <returns>Result of the operation</returns>
        [HttpPost("role")]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddMemberRole([FromBody] AddMemberRoleDto request)
        {
            _logger.LogInformation("POST api/User/role for userId: {userId}", request.UserId);
            if (request.Role != UserRole.Member)
            {
                // Only members can be added
                //adding other roles not implemented yet
                return BadRequest("Role must be Member");
            }
            await _userAdminService.AddMemberRole(request.UserId);
            return Ok();
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="userDto">Dto with required user information for registration</param>
        /// <returns>The id of the registered user</returns>
        [HttpPost("register")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationModelDto userDto)
        {
            _logger.LogInformation("POST api/User/register");
            var userId = await _userAdminService.RegisterUser(userDto);
            return CreatedAtAction(nameof(RegisterUser), userId);
        }

        /// <summary>
        /// Get users by FullName
        /// </summary>
        /// <param name="partialName">A part of a name to use as substring for filtering</param>
        /// <returns>List of users matching that match given partial name</returns>
        [HttpGet("names")]
        [Authorize(Roles = nameof(UserRole.Administrator))]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<UserNameDto>))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUsersByFullName(
            [FromQuery]
            string? partialName
            )
        {
            _logger.LogInformation("GET api/User/names?partialName={PartialName}", partialName);

            try
            {
                var users = await _userAdminService.GetUsersByFullName(partialName?.Trim());
                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieve user names with filter: {PartialName}", partialName);
                return BadRequest("Error retrieving user names");
            }
        }


        [HttpGet("count")]
        [Authorize(Roles = "Administrator")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(int))]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetActiveUsersCount()
        {
            _logger.LogInformation("GET api/User/count");
            try
            {
                var count = await _userAdminService.GetActiveUsersCountAsync();
                return Ok(count);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving active users count");
                return BadRequest("Error retrieving users count");
            }
        }

        /// <summary>
        /// Updates the profile of the logged in user
        /// </summary>
        /// <param name="userProfileDto">Dto with to be updated user information</param>
        /// <returns>No content</returns>
        [HttpPatch]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserProfileDto userProfileDto)
        {
            _logger.LogInformation("PATCH api/User/");
            await _userService.UpdateUserAsync(userProfileDto);
            return NoContent();
        }
    }
}
