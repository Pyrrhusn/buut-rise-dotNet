using Rise.Shared.Users;

namespace Rise.Client.Tests.Admins.Battery
{
    public class AdminBatteryPageTestAll : CustomAuthenticatedPageTest
    {
        private const string baseSuffix = "/admin/battery/";

        [Test]
        public async Task RedirectWhenNotLoggedIn()
        {
            await TestRedirectWhenNotLoggedIn(baseSuffix);
        }

        [Test]
        [TestCase(UserRole.Guest)]
        [TestCase(UserRole.Member)]
        public async Task NotAuthorized(UserRole role)
        {
            await TestNotAuthorized(baseSuffix, role);
        }
    }
}
