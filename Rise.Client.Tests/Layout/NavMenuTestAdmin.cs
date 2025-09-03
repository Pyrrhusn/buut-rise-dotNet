using System.Text.RegularExpressions;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Layout
{
    public class NavMenuTestAdmin : CustomAuthenticatedPageTest
    {
        private const int DefaultHeight = 1920;

        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Administrator);
        }

        [Test]
        [TestCase("nav-desktop-admin", "/admin", "")]
        public async Task Desktop_NavMenu(string testId, string resultSuffix, string startSuffix)
        {
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await NavigateToUrl(startSuffix);
            await Page.GetByTestId(testId).ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex($"{resultSuffix}(\\?.*)?$"));
        }

        [Test]
        [TestCase("nav-admin-guests", "/admin/guests", "")]
        [TestCase("nav-admin-cruise-period", "/admin/cruise_period", "")]
        [TestCase("nav-admin-battery", "/admin/battery", "")]
        public async Task Mobile_NavNotifications(string testId, string resultSuffix, string startSuffix)
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await NavigateToUrl(startSuffix);
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Page.GetByTestId(testId).ClickAsync();

            await Expect(Page).ToHaveURLAsync(new Regex($"{resultSuffix}$"));
        }


        /// <summary>
        /// Test to check if the admin dashboard is visible on the desktop version of the website.
        /// <returns></returns>
        [Test]
        public async Task CheckAdminDashBoardDesktop()
        {
            await Page.SetViewportSizeAsync(1080, 1920);
            await NavigateToUrl("/admin");

            await Expect(Page.GetByTestId("nav-desktop-admin").Nth(1)).ToBeVisibleAsync();
        }

    }
}