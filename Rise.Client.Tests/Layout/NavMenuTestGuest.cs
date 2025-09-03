using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Rise.Client.Tests.Notifications;
using Rise.Shared.Notifications;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Layout
{
    public class NavMenuTestGuest : CustomAuthenticatedPageTest
    {
        private const int DefaultHeight = 1920;

        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Guest);
        }

        public static readonly List<NotificationDto> Notifications = NotificationPageTestMember.Notifications;
        private static readonly int UnreadNotificationCount = Notifications.Where(n => !n.IsRead).Count();
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

            await Page.RouteAsync("*/**/api/Notification/me?*", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(Notifications.Take(3))
                });
            });

            await Page.RouteAsync("*/**/api/Notification/me/unread/count", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(Notifications.Where(n => !n.IsRead).Count())
                });
            });
        }

        [Test]
        [TestCase("nav-desktop-reservations", "/reservations", "")]
        // [TestCase("nav-desktop-book", "/book", "")]
        [TestCase("nav-desktop-profile", "/profile", "")]
        [TestCase("nav-desktop-logout", "/authentication/logout", "", false)]
        public async Task Desktop_NavMenu(string testId, string resultSuffix, string startSuffix, bool clickButton = true)
        {
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await NavigateToUrl(startSuffix);
            ILocator locator = Page.GetByTestId(testId);
            // TODO move logout to own test to prevent this silly logic
            if (clickButton)
            {
                await locator.ClickAsync();
                await Expect(Page).ToHaveURLAsync(new Regex($"{resultSuffix}(\\?.*)?$"));
            }
        }

        [Test]
        public async Task Desktop_NotificationsPopover_ToBeVisible()
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await NavigateToUrl("/home");

            ILocator notificationButton = Page.GetByTestId("nav-desktop-notifications");

            await Expect(notificationButton).ToBeVisibleAsync();
            await notificationButton.ClickAsync();

            ILocator popover = Page.GetByTestId("notifications-popover");
            await Expect(popover).ToBeVisibleAsync();
        }

        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public async Task Desktop_NotificationsPopover_ToHaveNotifications(int id)
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await NavigateToUrl("/home");

            ILocator notificationButton = Page.GetByTestId("nav-desktop-notifications");
            await notificationButton.ClickAsync();

            ILocator popoverList = Page.GetByTestId("notifications-popover-list");
            await Expect(popoverList).ToBeVisibleAsync();

            ILocator notification = Page.GetByTestId($"notification-{id}");
            await Expect(notification).ToBeVisibleAsync();
        }

        [Test]
        public async Task Desktop_NotificationsPopover_Button_ToBeVisible()
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await NavigateToUrl("/home");

            ILocator notificationButton = Page.GetByTestId("nav-desktop-notifications");
            await notificationButton.ClickAsync();

            ILocator popoverButton = Page.GetByTestId("notifications-popover-button");
            await Expect(popoverButton).ToBeVisibleAsync();
        }

        [Test]
        public async Task Desktop_NotificationsPopover_Button_ToNavigate()
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await NavigateToUrl("/home");

            ILocator notificationButton = Page.GetByTestId("nav-desktop-notifications");
            await notificationButton.ClickAsync();

            ILocator popoverButton = Page.GetByTestId("notifications-popover-button");
            await popoverButton.ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex("/notifications$"));
        }

        [Test]
        public async Task Desktop_NotificationsBadge_ToBeVisible()
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await NavigateToUrl("/home");

            ILocator notificationBadge = Page.GetByTestId("nav-desktop-notifications-count-badge");
            await Expect(notificationBadge).ToBeVisibleAsync();
        }

        [Test]
        public async Task Mobile_NotificationsBadge_ToBeVisible()
        {
            await MockHTTPRequests();
            await Page.SetViewportSizeAsync(959, 1920);
            await NavigateToUrl("/home");
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();

            ILocator notificationBadge = Page.GetByTestId("nav-mobile-notifications-count-badge");
            await Expect(notificationBadge).ToBeVisibleAsync();
        }

        [Test]
        [TestCase("nav-mobile-profile", "profile", "")]
        [TestCase("nav-mobile-reservations", "reservations", "")]
        [TestCase("nav-mobile-notifications", "notifications", "")]
        [TestCase("nav-mobile-profile", "profile", "")]
        [TestCase("nav-mobile-logout", "/authentication/logout", "", false)]
        public async Task Mobile_NavNotifications(string testId, string resultSuffix, string startSuffix, bool clickButton = true)
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await NavigateToUrl(startSuffix);
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            ILocator locator = Page.GetByTestId(testId);
            if (clickButton)
            {
                await locator.ClickAsync();
                await Expect(Page).ToHaveURLAsync(new Regex($"{resultSuffix}(\\?.*)?$"));
            }
        }

    }
}