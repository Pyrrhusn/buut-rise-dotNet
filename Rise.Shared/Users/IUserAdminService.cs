namespace Rise.Shared.Users;

public interface IUserAdminService
{
    Task<Pagination<UserDto>> GetUsersByRole(UserRole role, int page = 1, int pageSize = 10);
    Task<IEnumerable<UserNameDto>> GetUsersByFullName(string? partialName, CancellationToken token = default);
    Task<UserDetailDto> GetUserDetails(int userId);
    Task AddMemberRole(int userId);
    Task<int> RegisterUser(UserRegistrationModelDto userDto);
    Task<int> GetActiveUsersCountAsync();
}
