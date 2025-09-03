using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Rise.Shared.Users;

namespace Rise.Client.Admins;

public class UserAdminService(HttpClient httpClient) : IUserAdminService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<Pagination<UserDto>> GetUsersByRole(UserRole role, int page = 1, int pageSize = 10)
    {
        var result = await _httpClient.GetFromJsonAsync<Pagination<UserDto>>($"?role={role}&page={page}&pageSize={pageSize}");
        return result ?? new Pagination<UserDto>();
    }

    public async Task<UserDetailDto> GetUserDetails(int userId)
    {
        var result = await _httpClient.GetFromJsonAsync<UserDetailDto>(userId.ToString());
        return result ?? throw new Exception("Failed to get user details for user");
    }

    public async Task AddMemberRole(int userId)
    {
        await _httpClient.PostAsJsonAsync("role", new AddMemberRoleDto { UserId = userId, Role = UserRole.Member });
    }

    public Task<int> RegisterUser(UserRegistrationModelDto userDto)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UserNameDto>> GetUsersByFullName(string? partialName, CancellationToken cancellationToken = default)
    {
        Dictionary<string, string?> queries = [];

        if (partialName is not null)
        {
            queries["partialName"] = partialName.Trim();
        }

        string queryString = QueryHelpers.AddQueryString("names", queries);

        var result = await _httpClient.GetFromJsonAsync<IEnumerable<UserNameDto>>(queryString, cancellationToken: cancellationToken);
        return result ?? throw new Exception("Failed to get user names");
    }

    public async Task<int> GetActiveUsersCountAsync()
    {
        return await _httpClient.GetFromJsonAsync<int>("count");
    }
}