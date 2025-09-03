using System.Net.Http.Json;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Admins.CruisePeriods;

public class CruisePeriodService(HttpClient httpClient) : ICruisePeriodService
{
  private readonly HttpClient _httpClient = httpClient;

  public async Task<CruisePeriodDetailedDto> GetCruisePeriod(int id)
  {
    var response = await _httpClient.GetAsync(id.ToString());

    if (!response.IsSuccessStatusCode)
    {
      throw new Exception($"Failed to fetch cruise period with ID {id}. Response: {response.ReasonPhrase}");
    }

    return (await response.Content.ReadFromJsonAsync<CruisePeriodDetailedDto>())!;
  }

  public async Task<List<CruisePeriodDetailedDto>> GetCruisePeriods(bool getFuturePeriods)
  {
    var result = await _httpClient.GetFromJsonAsync<List<CruisePeriodDetailedDto>>($"?getFuturePeriods={getFuturePeriods}");
    return result ?? new List<CruisePeriodDetailedDto>();
  }
}