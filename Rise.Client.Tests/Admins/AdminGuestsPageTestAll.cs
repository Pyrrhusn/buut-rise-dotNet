using Rise.Shared.Users;

namespace Rise.Client.Tests.Admin
{
    public class AdminGuestsPageTestAll : CustomAuthenticatedPageTest
    {
        protected const string baseSuffix = "/admin/guests";

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