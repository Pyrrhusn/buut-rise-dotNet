using System.Net.Http.Json;
using Rise.Shared.Boats;

namespace Rise.Client.Admins.Battery
{
    public class BatteryService(HttpClient httpClient) : IBatteryService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<IEnumerable<BatteryDto>> GetBatteriesByBoat(int boatId)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<BatteryDto>>($"boat/{boatId}")
                ?? Array.Empty<BatteryDto>();
        }

        public async Task<BatteryDto> GetBattery(int id)
        {
            var response = await _httpClient.GetAsync(id.ToString());

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch battery with ID {id}. Response: {response.ReasonPhrase}");
            }

            return (await response.Content.ReadFromJsonAsync<BatteryDto>())!;
        }

        public async Task<BatteryDto> UpdateBattery(int id, BatteryUpdateDto newBattery)
        {
            var response = await _httpClient.PutAsJsonAsync(id.ToString(), newBattery);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update battery with ID {id}. Response: {response.ReasonPhrase}");
            }

            return (await response.Content.ReadFromJsonAsync<BatteryDto>())!;
        }
    }

}

