using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Rise.Client.Utils;
using Rise.Shared.TimeSlots;

namespace Rise.Client.TimeSlots
{
    public class TimeSlotService(HttpClient httpClient) : ITimeSlotService
    {
        private readonly HttpClient httpClient = httpClient;

        public async Task<int> CreateTimeSlot(CreateTimeSlotDto createTimeSlotsDto)
        {
            var result = await httpClient.PostAsJsonAsync("", createTimeSlotsDto);
            return await result.Content.ReadFromJsonAsync<int>();
        }

        public Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDate, DateOnly endDate)
        {
            Dictionary<string, string?> queries = new()
            {
                ["startDate"] = startDate.ToUniversalStringDate(),
                ["endDate"] = endDate.ToUniversalStringDate(),
            };

            string queryString = QueryHelpers.AddQueryString("range", queries);

            return httpClient.GetFromJsonAsync<TimeSlotRangeInfoDto>(queryString)!;
        }

        public Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(int year, int month, int day)
        {
            return httpClient.GetFromJsonAsync<IEnumerable<TimeSlotDto>>($"{year}/{month}/{day}")!;
        }
    }
}