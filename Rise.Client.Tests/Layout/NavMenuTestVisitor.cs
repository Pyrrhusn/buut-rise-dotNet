using System.Text.RegularExpressions;

namespace Rise.Client.Tests.Layout
{
    public class NavMenuTestVisitor : CustomPageTest
    {
        private const int DefaultHeight = 1920;

        [Test]
        [TestCase("nav-brand-logo", "/home", "/notFound")]
        [TestCase("nav-desktop-home", "/home", "/notFound")]
        [TestCase("nav-desktop-login", "/login", "")]
        public async Task Desktop_NavMenu(string testId, string resultSuffix, string startSuffix)
        {
            await Page.SetViewportSizeAsync(961, DefaultHeight);
            await NavigateToUrl(startSuffix);
            await Page.GetByTestId(testId).ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex($"{resultSuffix}(\\?.*)?$"));
        }

        [Test]
        [TestCase("nav-mobile-home", "home", "/huh")]
        [TestCase("nav-mobile-login", "/login", "")]

        public async Task Mobile_NavNotifications(string testId, string resultSuffix, string startSuffix)
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await NavigateToUrl(startSuffix);
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Page.GetByTestId(testId).ClickAsync();
            await Expect(Page).ToHaveURLAsync(new Regex($"{resultSuffix}(\\?.*)?$"));
        }


        /// <summary>
        /// Test to check if the change language button is visible on the desktop version of the website.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CheckLanguageChangeDesktop()
        {
            await Page.SetViewportSizeAsync(1080, 1920);
            await NavigateToUrl("/");

            await Page.GetByTestId("culture-selector-desktop").IsVisibleAsync();
        }


        /// <summary>
        /// Test to check if the change language button is visible on the mobile version of the website.
        /// </summary>
        /// <returns></returns>
        [Test]
        public async Task CheckLanguageChangeMobile()
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await NavigateToUrl("/");
            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Page.GetByTestId("culture-selector-mobile").IsVisibleAsync();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dutch"></param>
        /// <param name="english"></param>
        /// <returns></returns>
        [Test]
        [TestCase("nav-desktop-home", "HOME", "HOME")]
        public async Task ChangeLanguageBetweenLanguagesDesktop(string id, string dutch, string english)
        {
            await Page.SetViewportSizeAsync(1080, 1920);
            await NavigateToUrl("/");

            await Page.GetByTestId("culture-selector-desktop").First.ClickAsync();
            await Expect(Page.GetByTestId(id)).ToContainTextAsync(dutch);
            await Page.GetByTestId("en (US)").ClickAsync();
            await Hydration();

            await Expect(Page.GetByTestId(id)).ToContainTextAsync(english);
        }


        /// <summary>
        /// Test to check if the language changes between the languages on the mobile version of the website.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dutch"></param>
        /// <param name="english"></param>
        /// <returns></returns>
        [Test]
        [TestCase("nav-mobile-home", "HOME", "HOME")]
        public async Task ChangeLanguageBetweenLanguagesMobile(string id, string dutch, string english)
        {
            await Page.SetViewportSizeAsync(959, 1920);
            await NavigateToUrl("/");

            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Page.GetByTestId("culture-selector-mobile").First.ClickAsync();
            await Expect(Page.GetByTestId(id)).ToContainTextAsync(dutch);
            await Page.GetByTestId("en (US)").ClickAsync();
            await Hydration();

            await Page.GetByTestId("nav-drawer-open-button").ClickAsync();
            await Expect(Page.GetByTestId(id)).ToContainTextAsync(english);
        }
    }
}