using System.Text.Json;
using Rise.Shared.Notifications;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Notifications
{
    public class NotificationPageTestMember : CustomAuthenticatedPageTest // test for notifications that members will see/get
    {
        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Member);
        }

        public static readonly List<NotificationDto> Notifications =
            [
                new()
                {
                    Id = 1,
                    Severity = SeverityEnum.Success, // Success
                    Title = "Success",
                    Message = "This data comes from the seeding of the db.",
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                },
                new()
                {
                    Id = 2,
                    Severity = SeverityEnum.Info, // Info
                    Title = "Info",
                    Message = "This is an info message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(5)),
                    IsRead = false,
                },
                new()
                {
                    Id=3,
                    Severity = SeverityEnum.Warning, // Warning
                    Title = "Warning",
                    Message = "This is a warning message, but it is really long so it will be truncated",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)),
                    IsRead = false,
                },
                new()
                {
                    Id = 4,
                    Severity = SeverityEnum.Error, // Error
                    Title = "Error",
                    Message = "This is an error message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(5))),
                    IsRead = true,
                },
                new()
                {
                    Id = 5,
                    Severity = SeverityEnum.Error, // Error
                    Title = "Error",
                    Message = "This is an error message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(6))),
                    IsRead = false,
                },
                new()
                {
                    Id = 6,
                    Severity = SeverityEnum.Success, // Success
                    Title = "Deployment Complete",
                    Message = "Application successfully deployed to production environment",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    IsRead = true,
                },
                new()
                {
                    Id = 7,
                    Severity = SeverityEnum.Info, // Info
                    Title = "System Update",
                    Message = "Scheduled maintenance will occur tomorrow at 2 AM",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2).Add(TimeSpan.FromHours(3))),
                    IsRead = true,
                },
                new()
                {
                    Id = 8,
                    Severity = SeverityEnum.Warning, // Warning
                    Title = "Storage Alert",
                    Message = "Server storage capacity reaching 80%, consider cleanup",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    IsRead = true,
                },
                new()
                {
                    Id = 9,
                    Severity = SeverityEnum.Success, // Success
                    Title = "Backup Complete",
                    Message = "Weekly backup completed successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                    IsRead = true,
                },
                new()
                {
                    Id = 10,
                    Severity = SeverityEnum.Info, // Info
                    Title = "New Feature",
                    Message = "Dark mode is now available in your settings",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(5)),
                    IsRead = true,
                },
                new()
                {
                    Id = 11,
                    Severity = SeverityEnum.Error, // Error
                    Title = "Connection Failed",
                    Message = "Unable to connect to secondary database server",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(5).Add(TimeSpan.FromHours(6))),
                    IsRead = true,
                },
                new()
                {
                    Id = 12,
                    Severity = SeverityEnum.Warning, // Warning
                    Title = "CPU Usage High",
                    Message = "System CPU usage exceeded 90% for 5 minutes",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    IsRead = true,
                },
                new()
                {
                    Id = 13,
                    Severity = SeverityEnum.Success, // Success
                    Title = "Test Suite Passed",
                    Message = "All integration tests completed successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                    IsRead = true,
                },
                new()
                {
                    Id = 14,
                    Severity = SeverityEnum.Info, // Info
                    Title = "Profile Updated",
                    Message = "Your profile information has been updated successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                    IsRead = true,
                },
                new()
                {
                    Id = 15,
                    Severity = SeverityEnum.Warning, // Warning
                    Title = "SSL Certificate",
                    Message = "SSL Certificate will expire in 30 days",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                    IsRead = true,
                },
                new()
                {
                    Id = 16,
                    Severity = SeverityEnum.Error, // Error
                    Title = "Payment Failed",
                    Message = "Monthly subscription payment processing failed",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                    IsRead = true,
                },
                new()
                {
                    Id = 17,
                    Severity = SeverityEnum.Success, // Success
                    Title = "Report Generated",
                    Message = "Monthly analytics report has been generated",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(11)),
                    IsRead = true,
                },
                new()
                {
                    Id = 18,
                    Severity = SeverityEnum.Info, // Info
                    Title = "Team Meeting",
                    Message = "Reminder: Team meeting scheduled for tomorrow at 10 AM",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                    IsRead = true,
                },
                new()
                {
                    Id = 19,
                    Severity = SeverityEnum.Warning, // Warning
                    Title = "API Rate Limit",
                    Message = "API rate limit reached 85% of maximum allocation",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(13)),
                    IsRead = true,
                },
                new()
                {
                    Id = 20,
                    Severity = SeverityEnum.Error, // Error
                    Title = "Security Alert",
                    Message = "Multiple failed login attempts detected from unknown IP",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(14)),
                    IsRead = true,
                }
            ];

        private async Task MockHTTPRequests()
        {
            await Page.RouteAsync("*/**/api/Notification/me", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(Notifications)
                });
            });

            await Page.RouteAsync("*/**/api/Notification/read/*", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 204,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(new { })
                });
            });
        }

        [Test]
        public async Task HasNotificationsList()
        {
            await MockHTTPRequests();
            await NavigateToUrl("/notifications");
            await Page.GetByTestId("notifications-list").IsVisibleAsync();
        }

        [Test]
        public async Task HasNotificationDetails()
        {
            await MockHTTPRequests();
            await NavigateToUrl("/notifications");
            await Page.GetByTestId("notification-details").IsVisibleAsync();
        }

        [Test]
        [TestCase(1)]
        public async Task HasNotificationComponentWithCorrectData(int id)
        {
            await MockHTTPRequests();
            await NavigateToUrl("/notifications");

            DateTime timeStamp = Notifications.First(n => n.Id == id).CreatedAt;

            // Assert notification title
            var titleElement = Page.GetByTestId($"notification-title-{id}");
            await Expect(titleElement).ToHaveTextAsync("Success");

            // Assert notification message
            var messageElement = Page.GetByTestId($"notification-message-{id}");
            await Expect(messageElement).ToHaveTextAsync("This data comes from the seeding of the db.");

            // Assert notification timestamp
            var timestampElement = Page.GetByTestId($"notification-timestamp-{id}");
            await Expect(timestampElement).ToHaveTextAsync(timeStamp.ToString("t"));

            // Assert is-unread indicator is visible
            var isReadElement = Page.GetByTestId($"notification-is-unread-badge-{id}");
            await Expect(isReadElement).ToBeVisibleAsync();
        }

        [Test]
        [TestCase(1)]
        public async Task HasNoUnreadIndicatorWhenNotificationIsRead(int id)
        {
            await MockHTTPRequests();
            await NavigateToUrl("/notifications");

            // Assert is-unread indicator is visible
            var isReadElement = Page.GetByTestId($"notification-is-unread-badge-{id}");
            await Expect(isReadElement).ToBeVisibleAsync();

            // Click on the notification to mark it as read
            var notificationElement = Page.GetByTestId($"notification-{id}");
            await notificationElement.ClickAsync();

            // Assert is-unread indicator is not visible
            await Expect(isReadElement).Not.ToBeVisibleAsync();
        }

        [Test]
        [TestCase(1)]
        public async Task HasNotificationDetailsWhenNotificationClicked(int id)
        {
            await MockHTTPRequests();
            await NavigateToUrl("/notifications");

            // Click on the notification
            var notificationElement = Page.GetByTestId($"notification-{id}");
            await notificationElement.ClickAsync();

            // Assert notification details are visible
            await Page.GetByTestId("notification-details").IsVisibleAsync();

            // Assert notification title
            var titleElement = Page.GetByTestId("notification-details-title");
            await Expect(titleElement).ToHaveTextAsync("Success");

            // Assert notification message
            var messageElement = Page.GetByTestId("notification-details-message");
            await Expect(messageElement).ToHaveTextAsync("This data comes from the seeding of the db.");

            // Assert notification timestamp
            var timestampElement = Page.GetByTestId("notification-details-timestamp");
            await Expect(timestampElement).ToHaveTextAsync(Notifications.First(n => n.Id == id).CreatedAt.ToString("t"));
        }

        [Test]
        [TestCase(1, 2)]
        public async Task HasNoUnreadIndicatorWhenUnreadNotificationIsClicked(int id1, int id2)
        {
            await MockHTTPRequests();
            await NavigateToUrl("/notifications");

            // Assert is-unread indicator is visible
            var isReadElementBadge = Page.GetByTestId($"notification-is-unread-badge-{id1}");
            await Expect(isReadElementBadge).ToBeVisibleAsync();

            // Click on the notification
            var notificationElement1 = Page.GetByTestId($"notification-{id1}");
            await notificationElement1.ClickAsync();

            // Assert is-unread indicator is not visible
            await Expect(isReadElementBadge).Not.ToBeVisibleAsync();

            // Click on the notification
            var notificationElement2 = Page.GetByTestId($"notification-{id2}");
            await notificationElement2.ClickAsync();

            // Assert is-unread indicator is not visible
            await Expect(isReadElementBadge).Not.ToBeVisibleAsync();
        }
    }

}

