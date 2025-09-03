using Microsoft.Extensions.Configuration;
using Microsoft.Playwright;
using Rise.Shared.Users;
using System.Collections.Concurrent;

namespace Rise.Client.Tests
{
    public class CustomAuthenticatedPageTest : CustomPageTest
    {
        private static readonly ConcurrentDictionary<UserRole, SemaphoreSlim> roleSemaphores = new();
        private static readonly ConcurrentDictionary<UserRole, string?> roleSessionStorage = new();

        public static void Dispose()
        {
            foreach (var semaphore in roleSemaphores.Values)
            {
                semaphore.Dispose();
            }
        }

        protected async Task LoginAsync(UserRole role)
        {
            var semaphore = roleSemaphores.GetOrAdd(role, _ => new SemaphoreSlim(1, 1));

            await semaphore.WaitAsync();

            try
            {
                if (IsLoggedIn(role))
                {
                    roleSessionStorage.TryGetValue(role, out string? savedSession);
                    await LoadLoginFromSession(savedSession);
                }
                else
                {
                    Credentials? credentials = role switch
                    {
                        UserRole.Administrator => Configuration.GetSection("Administrator").Get<Credentials>(),
                        UserRole.Guest => Configuration.GetSection("Guest").Get<Credentials>(),
                        UserRole.Member => Configuration.GetSection("Member").Get<Credentials>(),
                        _ => null
                    } ?? throw new InvalidOperationException("Credentials cannot be null");

                    int attempts = 0;

                    while (attempts < 5 && !IsLoggedIn(role))
                    {
                        try
                        {
                            await LoginProcess(credentials);
                            await SaveSessionStorage(role);
                        }
                        catch (LoginFailedException e)
                        {
                            Console.WriteLine(e.ToString());
                            await Task.Delay(2 ^ (++attempts) * 1000);
                        }
                    }

                }
            }
            finally
            {
                semaphore.Release();
            }
        }

        private static bool IsLoggedIn(UserRole role)
        {
            return roleSessionStorage.TryGetValue(role, out string? savedSession) && savedSession != null;

        }

        private async Task LoginProcess(Credentials? credentials)
        {
            await NavigateToUrl("/authentication/login");
            try
            {
                if (credentials is not null)
                {
                    await Page.FillAsync("input[name='username']", credentials.Email);
                    await Page.FillAsync("input[name='password']", credentials.Password);
                    await Page.ClickAsync("button[type='submit']:not(.ulp-hidden-form-submit-button)");
                }
                await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                await Hydration();
                await Page.WaitForSelectorAsync("[data-testid=login-finishing]", new PageWaitForSelectorOptions() { State = WaitForSelectorState.Detached, Timeout = 0 });
            }
            catch (PlaywrightException e)
            {
                throw new LoginFailedException(e);
            }
        }
        private async Task SaveSessionStorage(UserRole role)
        {
            string sessionStorage = await Page.EvaluateAsync<string>("() => JSON.stringify(sessionStorage)");

            roleSessionStorage[role] = sessionStorage;
        }

        private async Task LoadLoginFromSession(string? sessionStorage)
        {
            if (sessionStorage == null) return;

            await NavigateToUrl("/");
            await Page.EvaluateAsync(@"storage => {
                if (window.location.hostname === 'localhost') {
                    const entries = JSON.parse(storage);
                    for (const [key, value] of Object.entries(entries)) {
                        window.sessionStorage.setItem(key, value);
                    }
                }
            }", sessionStorage);

            await Page.EvaluateAsync(@"key => {
                if (window.location.hostname === 'localhost') {
                    return window.sessionStorage.getItem(key);
                }
                return 'Failed to load :c';
            }", "oidc.user:https://rise-gent2.eu.auth0.com:8vJtbXg2FptHGmKrpFl1tZwhiXOJZ57l");
            await LoginProcess(null);
        }

        private class Credentials
        {
            public required string Email { get; set; }
            public required string Password { get; set; }
        }

        protected async Task LogoutAsync(UserRole role)
        {
            if (!IsLoggedIn(role)) return;

            await Page.SetViewportSizeAsync(1080, 1920);
            await NavigateToUrl("/");
            await Page.GetByTestId("nav-desktop-logout").ClickAsync();

        }

        protected async Task CheckRedirectedToLogin()
        {
            await Expect(Page.GetByText("Log in to Buut")).ToBeVisibleAsync();
        }

        protected async Task TestRedirectWhenNotLoggedIn(string url)
        {
            await NavigateToUrl(url);
            await CheckRedirectedToLogin();
        }

        protected async Task TestNotAuthorized(string url, UserRole role)
        {
            await LoginAsync(role);
            await NavigateToUrl(url);
            await Expect(Page.GetByTestId("unauthorized")).ToBeVisibleAsync();
            await LogoutAsync(role);
        }
    }

    internal class LoginFailedException(Exception e) : Exception("Login attempt failed", e);
}
