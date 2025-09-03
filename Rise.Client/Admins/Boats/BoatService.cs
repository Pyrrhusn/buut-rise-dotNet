using System.Net.Http.Json;
using Rise.Shared;
using Rise.Shared.Boats;

namespace Rise.Client.Services
{
    public class BoatService(HttpClient httpClient) : IBoatService
    {
        private readonly HttpClient _httpClient = httpClient;

        public async Task<int> GetActiveBoatsCountAsync()
        {
            var result = await _httpClient.GetFromJsonAsync<int>("count");
            return result;
        }

        /// <summary>
        /// Haalt alle boten op.
        /// </summary>
        /// <returns>Een lijst van boten.</returns>
        public async Task<IEnumerable<BoatDto>> GetAllBoatsAsync()
        {
            var response = await _httpClient.GetAsync("");

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to fetch boats. Response: {response.ReasonPhrase}");
            }

            return (await response.Content.ReadFromJsonAsync<List<BoatDto>>())!;
        }

        /// <summary>
        /// Update de beschikbaarheid van een boot.
        /// </summary>
        /// <param name="boatId">De ID van de boot.</param>
        /// <param name="isAvailable">De nieuwe beschikbaarheidsstatus.</param>
        public async Task UpdateBoatAvailabilityAsync(int boatId, bool isAvailable)
        {
            var content = JsonContent.Create(isAvailable);
            var response = await _httpClient.PatchAsync($"{boatId}/availability", content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to update availability for boat with ID {boatId}. Status: {response.StatusCode}, Response: {response.ReasonPhrase}");
            }
        }

        public async Task<int> CreateBoatAsync(CreateBoatDto createBoatDto){
            var response = await _httpClient.PostAsJsonAsync("", createBoatDto);

            if (!response.IsSuccessStatusCode){
                throw new Exception($"Failed to create boat. Status: {response.StatusCode}, Response: {response.ReasonPhrase}");
            }

            return await response.Content.ReadFromJsonAsync<int>();
        }
    }
}
