using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using Microsoft.Extensions.Configuration;


namespace Rise.Client.Tests
{
    [Parallelizable(ParallelScope.Self)]
    [TestFixture]
    public class CustomPageTest : PageTest
    {
        protected static IConfiguration Configuration { get; private set; } = default!;

        [OneTimeSetUp]
        public virtual void GlobalSetUp()
        {
            var builder = new ConfigurationBuilder()
            .AddUserSecrets<CustomAuthenticatedPageTest>()
            .AddEnvironmentVariables();
            Configuration = builder.Build();
            SetDefaultExpectTimeout(5_000);

        }

        [SetUp]
        public async Task Setup()
        {
            await Context.Tracing.StartAsync(new()
            {
                Title = $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}",
                Screenshots = true,
                Snapshots = true,
                Sources = true
            });

        }

        [TearDown]
        public async Task TearDown()
        {
            bool failed = TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Error
                || TestContext.CurrentContext.Result.Outcome == NUnit.Framework.Interfaces.ResultState.Failure;

            await Context.Tracing.StopAsync(new()
            {
                Path = failed ? Path.Combine(
                    TestContext.CurrentContext.WorkDirectory,
                    "playwright-traces",
                    $"{TestContext.CurrentContext.Test.ClassName}.{TestContext.CurrentContext.Test.Name}.zip"
                ) : null,
            });
        }

        public override BrowserNewContextOptions ContextOptions()
        {
            string baseUrl = TestContext.Parameters.Get("BASE_URL", "https://localhost:5003");
            return new()
            {
                Locale = "en-US",
                ColorScheme = ColorScheme.Light,
                BaseURL = baseUrl,
                IgnoreHTTPSErrors = true,
            };
        }

        protected async Task Hydration()
        {
            await Page.WaitForSelectorAsync("[data-testid=app-loader]", new PageWaitForSelectorOptions() { State = WaitForSelectorState.Detached, Timeout = 0 });
            await Page.WaitForSelectorAsync("[data-testid=authorization-loader]", new PageWaitForSelectorOptions() { State = WaitForSelectorState.Detached, Timeout = 0 });
        }

        protected async Task NavigateToUrl(string url)
        {
            await Page.GotoAsync(url);
            await Hydration();
        }

        protected async Task ReloadPage()
        {
            await Page.ReloadAsync();
            await Hydration();
        }

    }
}