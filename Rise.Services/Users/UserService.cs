
using Rise.Persistence;
using Microsoft.EntityFrameworkCore;
using Rise.Shared.Users;
using DomainUser = Rise.Domain.Users.User;
using Rise.Domain.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Npgsql;
using Rise.Services.Constants;
using Auth0.Core.Exceptions;
using Microsoft.Extensions.Logging;
using Auth0.ManagementApi.Paging;
using static Rise.Shared.Users.UserRegistrationModelDto;
using System.Linq.Expressions;
using Rise.Services.Auth;
using Rise.Shared.Address;


namespace Rise.Services.Users;

public class UserService(
        ApplicationDbContext dbContext,
        IManagementApiClient managementApiClient,
        ILogger<UserService> logger,
        IAuthContextProvider authContextProvider
    )
    : AuthenticatedService(dbContext, authContextProvider),
      IUserAdminService,
      IUserRegisterService,
      IUserService
{
    private readonly IManagementApiClient _managementApiClient = managementApiClient;
    private readonly ILogger<UserService> _logger = logger;

    private async Task<Role> GetAuth0RoleByName(UserRole userRole)
    {
        var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = userRole.ToString() });
        return roles.FirstOrDefault() ?? throw new RoleNotFoundException($"Role '{userRole}' not found in Auth0.");
    }

    private async Task RemoveRoleFromUser(User auth0User, Role role)
    {
        await _managementApiClient.Users.RemoveRolesAsync(auth0User.UserId, new AssignRolesRequest
        {
            Roles = [role.Id]
        });
    }

    private async Task AssignRoleToUser(User auth0User, Role role)
    {
        await _managementApiClient.Users.AssignRolesAsync(auth0User.UserId, new AssignRolesRequest
        {
            Roles = [role.Id]
        });
    }
    public async Task<Pagination<UserDto>> GetUsersByRole(UserRole role, int page = 1, int pageSize = 10)
    {
        try
        {
            var auth0Role = await GetAuth0RoleByName(role);

            // Get paginated users from Auth0
            var assignedUsersPage = await GetAuth0UsersWithRetry(auth0Role.Id, page, pageSize);

            if (assignedUsersPage.Count == 0)
            {
                _logger.LogInformation("No users found for role {Role}", role);
                return CreateEmptyPaginationResult<UserDto>(page, pageSize);
            }

            var auth0Users = await Task.WhenAll(assignedUsersPage.Select(user =>
                _managementApiClient.Users.GetAsync(user.UserId)));

            var buutUserIds = auth0Users
                .Select<User, string?>(auth0User => auth0User.AppMetadata?["buutUserId"]?.ToString())
                .Where(id => !string.IsNullOrEmpty(id))
                .Distinct()
                .ToList();

            if (buutUserIds.Count == 0)
            {
                _logger.LogWarning("No valid buutUserId found in Auth0 users for role {Role}", role);
                return CreateEmptyPaginationResult<UserDto>(page, pageSize);
            }

            // Get users from our database matching the paginated Auth0 users
            var userDtos = await _dbContext.Users
                .Where(user => buutUserIds.Contains(user.Id.ToString()))
                .Select(u => new UserDto
                {
                    Id = u.Id,
                    FamilyName = u.FamilyName
                })
                .ToListAsync();

            return new Pagination<UserDto>
            {
                Items = userDtos,
                TotalCount = assignedUsersPage.Paging.Total,
                Page = page,
                PageSize = pageSize
            };
        }
        catch (RoleNotFoundException)
        {
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while fetching users by role {Role}", role);
            throw new ApplicationException($"Failed to retrieve users for role '{role}'.", ex);
        }
    }

    private async Task<IPagedList<AssignedUser>> GetAuth0UsersWithRetry(string roleId, int page, int pageSize)
    {
        const int maxRetries = 5;
        const int retryDelayInMilliseconds = 2000;
        int retries = 0;
        bool success = false;

        while (retries <= maxRetries && !success)
        {
            try
            {
                // Auth0 uses 0-based indexing for pages
                var assignedUsersPage = await _managementApiClient.Roles.GetUsersAsync(roleId,
                    new PaginationInfo(page - 1, pageSize, includeTotals: true));

                return assignedUsersPage;
            }
            catch (RateLimitApiException ex)
            {
                HandleRetryableException(ex, "Rate limit exceeded", roleId, retries, maxRetries);
            }
            catch (HttpRequestException ex)
            {
                HandleRetryableException(ex, $"HTTP request failed. Status: {ex.StatusCode}", roleId, retries, maxRetries);
            }
            catch (ErrorApiException ex)
            {
                HandleRetryableException(ex, $"Auth0 API error. Status: {ex.StatusCode}, Error: {ex.Message}", roleId, retries, maxRetries);
            }
            catch (ApiException ex)
            {
                HandleRetryableException(ex, "Unexpected Auth0 API error", roleId, retries, maxRetries);
            }

            await Task.Delay(retryDelayInMilliseconds);
            retries++;
        }

        throw new ApplicationException($"Failed to fetch users for role '{roleId}' after {maxRetries + 1} attempts.");
    }

    private void HandleRetryableException(Exception ex, string message, string roleId, int retries, int maxRetries)
    {
        if (retries >= maxRetries)
        {
            throw new ApplicationException($"Failed to fetch users for role '{roleId}' after {maxRetries + 1} attempts.", ex);
        }

        _logger.LogWarning(ex, "{Message} while fetching users for role {RoleId}. Attempt {Attempt} of {MaxAttempts}.",
            message, roleId, retries + 1, maxRetries + 1);
    }

    private static Pagination<T> CreateEmptyPaginationResult<T>(int page, int pageSize)
    {
        return new Pagination<T>
        {
            Items = [],
            TotalCount = 0,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task<UserDetailDto> GetUserDetails(int userId)
    {
        DomainUser user = await _dbContext.Users
            .Include(u => u.Address)
            .FirstOrDefaultAsync(u => u.Id == userId)
            ?? throw new EntityNotFoundException(nameof(DomainUser), userId);


        return new UserDetailDto
        {
            Id = user.Id,
            FirstName = user.FirstName,
            FamilyName = user.FamilyName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            Address = new AddressModel
            {
                Street = user.Address.Street,
                Number = user.Address.Number,
                City = user.Address.City,
                PostalCode = user.Address.PostalCode,
                Country = user.Address.Country
            }
        };

    }
    private async Task<Auth0.ManagementApi.Models.User?> FindUserByBuutUserId(int buutUserId)
    {
        // Fetch Auth0 by buutUserId in app_metadata
        var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest() { Query = $"app_metadata.buutUserId:{buutUserId}" });
        return users.FirstOrDefault();
    }
    public async Task AddMemberRole(int userId)
    {
        var auth0User = await FindUserByBuutUserId(userId) ?? throw new EntityNotFoundException("Auth0 user", userId);

        try
        {
            await RemoveRoleFromUser(auth0User, await GetAuth0RoleByName(UserRole.Guest));
            await AssignRoleToUser(auth0User, await GetAuth0RoleByName(UserRole.Member));
        }
        catch (ErrorApiException ex)
        {
            _logger.LogError(ex, "Auth0 error");
            throw new RoleAssigningFailedException(ex.Message);
        }
        catch (RateLimitApiException ex)
        {
            _logger.LogError(ex, "Auth0 Rate limit exceeded");
            throw new RoleAssigningFailedException(ex.Message);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Auth0 api exception");
            throw new RoleAssigningFailedException(ex.Message);
        }
    }

    public async Task<int> RegisterUser(UserRegistrationModelDto userDto)
    {
        using var transaction = _dbContext.Database.BeginTransaction();

        transaction.CreateSavepoint("BeforeSavingUser");

        var user = await CreateUserInDatabase(userDto);

        await RegisterUserInAuth0(userDto, user.Id);

        transaction.Commit(); //Commit transaction after user is registered in auth0 so if it fails, user is rolled back in our db.

        return user.Id;
    }

    private async Task<DomainUser> CreateUserInDatabase(UserRegistrationModelDto userDto)
    {
        var address = userDto.Address;
        DomainUser user = new()
        {
            Email = userDto.Email,
            FirstName = userDto.FirstName,
            FamilyName = userDto.FamilyName,
            PhoneNumber = userDto.PhoneNumber,
            DateOfBirth = (DateTime) userDto.DateOfBirth!,
            Address = new()
            {
                City = address.City,
                Country = address.Country,
                Number = address.Number,
                PostalCode = address.PostalCode,
                Street = address.Street
            }
        };

        try
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(ex, "Error while saving user.");
            HandleDbUpdateException(ex);
            throw new UserCreationFailedException(ErrorMessages.User.UnexpectedError);
        }

        return user;
    }

    private async Task RegisterUserInAuth0(UserRegistrationModelDto userDto, int userId)
    {
        try
        {
            var isSuccess = await RunTaskWithRetries(async () => await SendRegisterUserInAuth0Request(userDto, userId), 20);

            if (!isSuccess)
            {
                throw new UserCreationFailedException(ErrorMessages.User.Auth0RateLimitExceeded);
            }
        }
        catch (ErrorApiException ex)
        {
            _logger.LogError(ex, "Auth0 error");
            if (ex.Message.Contains("already exists"))
                throw new UniqueConstraintViolationException(ErrorMessages.User.EmailAlreadyExists);
            throw new UserCreationFailedException(ex.Message);
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Auth0 api exception");
            throw new UserCreationFailedException(ex.Message);
        }
    }

    private async Task<bool> SendRegisterUserInAuth0Request(UserRegistrationModelDto userDto, int userId)
    {
        try
        {
            var auth0User = await _managementApiClient.Users.CreateAsync(new UserCreateRequest
            {
                UserName = userDto.Email,
                Email = userDto.Email,
                Connection = "Username-Password-Authentication",
                Password = userDto.Password,
                AppMetadata = new Dictionary<string, object> {
                    { "buutUserId", userId },
                }
            });

            await RunTaskWithRetries(async () => await SendAssignInitialRoleRequest(auth0User), 20);

            return true;
        }
        catch (RateLimitApiException ex)
        {
            //Delay so that auth0 api doesn't throw a rate limit exception
            _logger.LogError(ex, "Rate limit exceeded.");
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Http timeout exceeded.");
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        return false;
    }

    private async Task<bool> SendAssignInitialRoleRequest(User auth0User)
    {
        try
        {
            var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = nameof(UserRole.Guest) });
            var role = roles.FirstOrDefault() ?? throw new RoleNotFoundException(nameof(UserRole.Guest));
            await _managementApiClient.Users.AssignRolesAsync(auth0User.UserId, new AssignRolesRequest
            {
                Roles = [role.Id]
            });
            return true;
        }
        catch (RateLimitApiException ex)
        {
            //Delay so that auth0 api doesn't throw a rate limit exception
            _logger.LogError(ex, "Rate limit exceeded.");
            await Task.Delay(TimeSpan.FromSeconds(2));
        }
        catch (TaskCanceledException ex)
        {
            _logger.LogError(ex, "Http timeout exceeded.");
            await Task.Delay(TimeSpan.FromSeconds(5));
        }
        return false;
    }

    private static async Task<bool> RunTaskWithRetries(Func<Task<bool>> callback, int retryLimit)
    {
        var retries = 0;
        var isSuccess = false;
        while (retries <= retryLimit && !isSuccess)
        {
            isSuccess = await callback();
            retries++;
        }
        return isSuccess;
    }

    private static void HandleDbUpdateException(DbUpdateException ex)
    {
        if (ex.InnerException is PostgresException pgEx)
        {
            string message = pgEx.ConstraintName switch
            {
                DatabaseConstraints.UniqueUserEmail => ErrorMessages.User.EmailAlreadyExists,
                _ => ErrorMessages.User.UnexpectedError
            };
            throw new UniqueConstraintViolationException(message);
        }
    }

    public async Task<IEnumerable<UserNameDto>> GetUsersByFullName(string? partialName, CancellationToken cancellationToken = default)
    {
        try
        {
            Expression<Func<DomainUser, bool>> filterName = !string.IsNullOrWhiteSpace(partialName) ? (u) => u.FullName.ToLower().Contains(partialName.ToLower()) : (u) => true;

            int totalCount = await _dbContext.Users.Where(filterName).CountAsync(cancellationToken);
            if (totalCount == 0)
            {
                _logger.LogWarning("No users found that contain: {partialName}", partialName);
                return [];
            }

            List<UserNameDto> userDtos = await _dbContext.Users
                .Where(filterName)
                .Select(u => new UserNameDto
                {
                    Id = u.Id,
                    FirstName = u.FirstName,
                    FamilyName = u.FamilyName,
                    FullName = u.FullName,
                })
                .OrderBy(u => u.FullName)
                .AsNoTracking()
                .ToListAsync();

            return userDtos;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while retrieve user names with filter '{partialName}'", partialName);
            throw new ApplicationException($"Failed to retrieve retrieve user names with filter '{partialName}'", ex);
        }
    }

    /// <summary>
    /// Gets the number of active users in the system.
    /// </summary>
    /// <returns>Amount of active users in the system</returns>
    public async Task<int> GetActiveUsersCountAsync()
    {
        return await _dbContext.Users
            .CountAsync(u => !u.IsDeleted);
    }

    public async Task UpdateUserAsync(UpdateUserProfileDto userProfileDto)
    {
        int userId = (int)_authContextProvider.GetUserId()!;

        var user = await _dbContext.Users.FindAsync(userId) ?? throw new EntityNotFoundException(nameof(DomainUser), userId);

        user.FirstName = userProfileDto.FirstName;
        user.FamilyName = userProfileDto.FamilyName;
        user.PhoneNumber = userProfileDto.PhoneNumber;
        user.Address = AddressDtoToUserAddress(userProfileDto.Address);

        await _dbContext.SaveChangesAsync();
    }

    public async Task<UserProfileDto> GetUserProfile()
    {
        int userId = (int)_authContextProvider.GetUserId()!;

        var user = await _dbContext.Users.FindAsync(userId) ?? throw new EntityNotFoundException(nameof(DomainUser), userId);

        return new()
        {
            DateOfBirth = user.DateOfBirth,
            Email = user.Email,
            Address = UserAddressToAddressDto(user.Address),
            FamilyName = user.FamilyName,
            FirstName = user.FirstName,
            PhoneNumber = user.PhoneNumber,
        };
    }

    private static DomainUser.UserAddress AddressDtoToUserAddress(AddressDto addressDto)
    {
        return new()
        {
            City = addressDto.City,
            Country = addressDto.Country,
            Number = addressDto.Number,
            PostalCode = addressDto.PostalCode,
            Street = addressDto.Street,
        };
    }

    private static AddressDto UserAddressToAddressDto(DomainUser.UserAddress address)
    {
        return new()
        {
            City = address.City,
            Country = address.Country,
            Number = address.Number,
            PostalCode = address.PostalCode,
            Street = address.Street,
        };
    }
}


