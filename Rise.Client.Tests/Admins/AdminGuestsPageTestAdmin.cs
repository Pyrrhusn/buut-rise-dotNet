using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Admin
{

    public class AdminGuestsPageTestAdmin : CustomAuthenticatedPageTest
    {
        protected const string baseSuffix = "/admin/guests";

        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Administrator);
        }

        private async Task MockUsers(int page = 1, int pageSize = 10, bool hasNextPage = false)

        {
            var users = new UserDto[]
            {
                new UserDto
                {
                    Id = 1,
                    FamilyName = "Smith"
                },
                new UserDto
                {
                    Id = 2,
                    FamilyName = "Johnson"
                }
            };

            var response = new Pagination<UserDto>
            {
                Items = users,
                Page = page,
                PageSize = pageSize,
                TotalCount = users.Length,
                HasNextPage = hasNextPage
            };

            await Page.RouteAsync("**/api/User?**", async route =>
            {

                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = JsonSerializer.Serialize(response)
                });
            });
        }

        private async Task InitializeWithMockUsers(int page = 1, bool hasNextPage = false)
        {
            await MockUsers(page, 10, hasNextPage);
            await NavigateToUrl(baseSuffix);
        }

        [Test]
        public async Task DisplaysUsersTable()
        {
            await InitializeWithMockUsers();
            var locator = Page.GetByTestId("users-table");
            await Expect(locator).ToBeVisibleAsync();
        }

        [Test]
        public async Task DisplaysNoUsersMessageWhenEmpty()
        {
            var emptyResponse = new Pagination<UserDto>
            {
                Items = Array.Empty<UserDto>(),
                Page = 1,
                PageSize = 10,
                TotalCount = 0,
                HasNextPage = false
            };

            await Page.RouteAsync("**/api/User?**", async route =>
            {
                await Task.Delay(2000);

                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = JsonSerializer.Serialize(emptyResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
                });
            });

            await NavigateToUrl(baseSuffix);
            await Page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await Expect(Page.GetByTestId("users-none")).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShowsLoadingStateWhileFetchingUsers()
        {
            var emptyResponse = new Pagination<UserDto>
            {
                Items = Array.Empty<UserDto>(),
                Page = 1,
                PageSize = 10,
                TotalCount = 0,
                HasNextPage = false
            };

            await Page.RouteAsync("**/api/User?**", async route =>
            {
                await Task.Delay(1000);
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "application/json",
                    Body = JsonSerializer.Serialize(emptyResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    })
                });
            });

            await NavigateToUrl(baseSuffix);
            await Expect(Page.GetByTestId("user-list-loading-progress")).ToBeVisibleAsync();
        }

        [Test]
        public async Task TableHasCorrectHeaders()
        {
            await InitializeWithMockUsers();
            await Expect(Page.GetByTestId("users-list-familyName")).ToContainTextAsync("Familienaam");
        }

        [Test]
        public async Task DisplaysUserFamilyNames()
        {
            await InitializeWithMockUsers();
            await AssertUserDetails("list-guest-page-familyname-1", "Smith");
            await AssertUserDetails("list-guest-page-familyname-2", "Johnson");
        }

        private async Task AssertUserDetails(string testId, string expectedValue)
        {
            var locator = Page.GetByTestId(testId);
            await Expect(locator).ToBeVisibleAsync();
            await Expect(locator).ToHaveTextAsync(expectedValue);
        }
    }
}