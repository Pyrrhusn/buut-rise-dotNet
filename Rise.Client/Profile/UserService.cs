using System;
using System.Net;
using System.Net.Http.Json;
using Rise.Shared.Users;

namespace Rise.Client.Profile;

public class UserService(HttpClient httpClient) : IUserService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<UserProfileDto> GetUserProfile()
    {
        var userProfileDto =
            await _httpClient.GetFromJsonAsync<UserProfileDto>("profile") ?? throw new Exception("Failed to get user profile");
        return userProfileDto;
    }

    public async Task UpdateUserAsync(UpdateUserProfileDto updateUserProfileDto)
    {
        var response = await _httpClient.PatchAsJsonAsync("", updateUserProfileDto);
        if (response.StatusCode != HttpStatusCode.NoContent)
        {
            throw new Exception($"Failed to update profile: Status: {response.StatusCode}, Response: {response.ReasonPhrase}");
        }
    }
}
