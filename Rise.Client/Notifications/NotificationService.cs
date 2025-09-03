using System;
using System.Net.Http.Json;
using Microsoft.AspNetCore.WebUtilities;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications
{
    public class NotificationService(HttpClient httpClient) : INotificationService
    {
        private readonly HttpClient _httpClient = httpClient;

        public Task<int> GetUnreadNotificationCount()
        {
            return _httpClient.GetFromJsonAsync<int>("me/unread/count");
        }

        public async Task<IEnumerable<NotificationDto>> GetUserNotifications(int? limit)
        {

            Dictionary<string, string?> queries = [];

            if (limit.HasValue)
                queries.Add("limit", limit.ToString());

            string queryString = QueryHelpers.AddQueryString("me", queries);

            IEnumerable<NotificationDto> result = await _httpClient.GetFromJsonAsync<IEnumerable<NotificationDto>>(queryString)
                ?? throw new Exception("Failed to get notifications");
            return result;
        }

        public Task MarkNotificationAsRead(int notificationId)
        {
            return _httpClient.PatchAsync($"read/{notificationId}", null);
        }

    }

}

