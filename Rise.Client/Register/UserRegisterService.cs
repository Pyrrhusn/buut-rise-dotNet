using System.Net.Http.Json;
using System.Text.Json;
using Rise.Shared.Users;
using Serilog;

namespace Rise.Client.Register;

public class UserRegisterService(HttpClient httpClient) : IUserRegisterService
{
    private readonly HttpClient _httpClient = httpClient;

    public async Task<int> RegisterUser(UserRegistrationModelDto userDto)
    {
        try
        {
            var result = await _httpClient.PostAsJsonAsync("", userDto);

            if (!result.IsSuccessStatusCode)
            {
                throw new Exception($"{result.ReasonPhrase}:\n{await result.Content.ReadAsStringAsync()}");
            }

            return await result.Content.ReadFromJsonAsync<int>();
        }
        catch (Exception)
        {
            throw;
        }
    }
}
