using System.Text.Json;
using Rise.Shared.Users;
using static Rise.Shared.Users.UserRegistrationModelDto;

namespace Rise.Client.Tests.Admin
{
    public class AdminGuestDetailsPageTestAdmin : CustomAuthenticatedPageTest
    {
        protected const string baseSuffix = "/admin/guests";

        private readonly static string[] fieldNames = ["name", "email", "address", "phone"];


        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Administrator);
        }


        private async Task AssertUserDetail(string testId, string expectedValue)
        {
            var locator = Page.GetByTestId(testId);
            await Expect(locator).ToBeVisibleAsync();
            await Expect(locator).ToHaveTextAsync(expectedValue);
        }

        private async Task MockUserDetails(int userId, UserDetailDto userDetails, int? delayMs = null)
        {
            await Page.RouteAsync($"**/api/User/{userId}", async route =>
            {
                if (delayMs.HasValue)
                {
                    await Task.Delay(delayMs.Value);
                }

                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(userDetails)
                });
            });
        }

        private async Task InitializeWithMockUser(int userId = 1, int? delayMs = null)
        {
            var userDetails = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Smith",
                FirstName = "John",
                Email = "john.smith@example.com",
                PhoneNumber = "+1234567890",
                Address = new AddressModel
                {
                    Street = "Main Street",
                    Number = "123",
                    City = "New York",
                    PostalCode = "12345",
                    Country = "USA"
                }
            };

            await MockUserDetails(userId, userDetails, delayMs);
            await NavigateToUrl($"{baseSuffix}/{userId}");
        }

        [Test]
        public async Task DisplaysUserFamilyName()
        {
            await InitializeWithMockUser(1);
            await AssertUserDetail("user-details-page-familyname", "John Smith");

        }
        [Test]
        public async Task DisplaysUserPhone()
        {
            await InitializeWithMockUser(1);
            await AssertUserDetail("user-details-page-phone", "+1234567890");
        }

        [Test]
        public async Task DisplaysUserAddress()
        {
            await InitializeWithMockUser(1);
            await AssertUserDetail("user-details-page-address", "Main Street 123, 12345 New York, USA");
        }

        [Test]
        public async Task ShowsLoadingStateWhileFetchingDetails()
        {
            await InitializeWithMockUser(1, 5000);
            await Expect(Page.GetByTestId("user-details-loading-progress")).ToBeVisibleAsync();
        }

        [Test]
        public async Task DisplaysErrorMessageOnFailedFetch()
        {
            const int userId = 1;
            await Page.RouteAsync($"**/api/User/{userId}", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 500,
                    ContentType = "text/plain",
                    Body = "Internal Server Error"
                });
            });

            await NavigateToUrl($"{baseSuffix}/{userId}");
            await Expect(Page.GetByTestId("user-details-fetch-error")).ToBeVisibleAsync();
        }

        [Test]
        public async Task NavigatesBackToListOnButtonClick()
        {
            await InitializeWithMockUser(1);

            await Page.GetByTestId("back-to-guests-list-button").ClickAsync();
            await Expect(Page).ToHaveURLAsync(baseSuffix);
        }
        [Test]
        public async Task UpdatesAllFieldsOnDataRefresh()
        {
            const int userId = 1;
            // Initial state
            var initialUser = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Smith",
                FirstName = "John",
                Email = "john.smith@example.com",
                PhoneNumber = "+1234567890",
                Address = new AddressModel
                {
                    Street = "Main Street",
                    Number = "123",
                    City = "New York",
                    PostalCode = "12345",
                    Country = "USA"
                }
            };

            await MockUserDetails(userId, initialUser);
            await NavigateToUrl($"{baseSuffix}/{userId}");

            // Verify initial state
            await AssertUserDetail("user-details-page-name", "John Smith");
            await AssertUserDetail("user-details-page-email", "john.smith@example.com");
            await AssertUserDetail("user-details-page-phone", "+1234567890");
            await AssertUserDetail("user-details-page-address", "Main Street 123, 12345 New York, USA");

            // Updated state
            var updatedUser = new UserDetailDto
            {
                Id = userId,
                FamilyName = "Johnson",
                FirstName = "John",
                Email = "john.johnson@example.com",
                PhoneNumber = "+1987654321",
                Address = new AddressModel
                {
                    Street = "Broadway",
                    Number = "456",
                    City = "Los Angeles",
                    PostalCode = "90001",
                    Country = "USA"
                }
            };

            await MockUserDetails(userId, updatedUser);
            await ReloadPage();

            // Verify updated state
            await AssertUserDetail("user-details-page-name", "John Johnson");
            await AssertUserDetail("user-details-page-email", "john.johnson@example.com");
            await AssertUserDetail("user-details-page-phone", "+1987654321");
            await AssertUserDetail("user-details-page-address", "Broadway 456, 90001 Los Angeles, USA");
        }

        [Test]
        public async Task HandlesInvalidUserIdGracefully()
        {
            const int invalidUserId = 999;
            await Page.RouteAsync($"**/api/User/{invalidUserId}", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 404,
                    ContentType = "text/plain",
                    Body = "User not found"
                });
            });

            await NavigateToUrl($"{baseSuffix}/{invalidUserId}");
            await Expect(Page.GetByTestId("user-details-fetch-error")).ToBeVisibleAsync();
        }


        [Test]
        public async Task DisplaysAllUserFields()
        {
            await InitializeWithMockUser(1);
            await Task.WhenAll(fieldNames
                .Select(field => Expect(Page.GetByTestId($"user-details-page-{field}")).ToBeVisibleAsync()));
        }
    }
}