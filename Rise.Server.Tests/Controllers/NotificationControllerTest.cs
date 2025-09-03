using System;
using System.Net;
using System.Net.Http.Json;
using Rise.Server.Tests.Fixtures;
using Rise.Shared.Notifications;
using Rise.Shared.Users;
using Shouldly;

namespace Rise.Server.Tests.Controllers
{
    public class NotificationControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "Notification")

    {
        [Fact]
        public async Task GET_CurrentUser_Notifications_ReturnsOk()
        {
            await LoginAsync(UserRole.Member);
            var response = await _client.GetAsync("me");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
        }

        [Fact]
        public async Task GET_CurrentUser_Notifications_Unread_Count_ReturnsCount()
        {
            await LoginAsync(UserRole.Member);
            var response = await _client.GetAsync("me/unread/count");
            var count = await response.Content.ReadFromJsonAsync<int>();
            count.ShouldBeGreaterThanOrEqualTo(1);
        }

        [Fact]
        public async Task GET_CurrentUser_Notifications_GivesNotifications()
        {
            await LoginAsync(UserRole.Member);
            IEnumerable<NotificationDto> response = (await _client.GetFromJsonAsync<IEnumerable<NotificationDto>>("me"))!;
            response.Count().ShouldBeGreaterThanOrEqualTo(20);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public async Task PATCH_CurrentUser_Notifications_MarkAsRead(int id)
        {
            await LoginAsync(UserRole.Member);
            var response = await _client.PatchAsync($"read/{id}", null);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Theory]
        [InlineData(100)]
        public async Task PATCH_CurrentUser_Notifications_MarkAsRead_NotFound(int id)
        {
            await LoginAsync(UserRole.Member);
            var response = await _client.PatchAsync($"read/{id}", null);
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GET_CurrentUser_Notifications_DoesNotGiveNotificationsFromOtherUsers()
        {
            await LoginAsync(UserRole.Member);
            // These are the ids of the notifications that are not supposed to be returned
            var forbiddenIds = new[] { 21, 22, 23, 24, 25, 26, 27 };

            IEnumerable<NotificationDto> response = (await _client.GetFromJsonAsync<IEnumerable<NotificationDto>>("me"))!;

            response.Any(n => forbiddenIds.Contains(n.Id)).ShouldBeFalse();
        }
    }

}

