
using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.Core.Exceptions;
using Auth0.ManagementApi;
using Auth0.ManagementApi.Models;
using Microsoft.Extensions.DependencyInjection;
using Rise.Server.Tests.Utils;
using Rise.Shared.Users;
using Shouldly;

namespace Rise.Server.Tests.Fixtures
{
    [Trait("Category", "Integration")]
    public abstract class IntegrationTest : IClassFixture<ApiWebApplicationFactory>, IAsyncLifetime
    {

        // private readonly Checkpoint _checkpoint = new()
        // {
        //     SchemasToInclude = [
        //         "Boat",
        //         "CruisePeriods",
        //         "Reservation",
        //         "ReservationUser",
        //         "TimeSlots",
        //         "User",
        // ],
        //     DbAdapter = DbAdapter.Postgres,
        //     WithReseed = true
        // };

        private static readonly ConcurrentDictionary<UserRole, SemaphoreSlim> roleSemaphores = new();
        private static readonly ConcurrentDictionary<UserRole, string?> roleSessionStorage = new();
        protected readonly ApiWebApplicationFactory _factory;
        protected readonly HttpClient _client;
        private readonly AuthenticationApiClient _authenticationApiClient;
        private readonly IManagementApiClient _managementApiClient;
        private readonly List<string> _createdUserIds = [];

        public IntegrationTest(ApiWebApplicationFactory fixture, string routeBase)
        {
            _factory = fixture;
            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri(_client.BaseAddress ?? new Uri("https://localhost"), "api/" + routeBase + "/");
            var config = fixture.Configuration;

            _managementApiClient = fixture.Services.GetRequiredService<IManagementApiClient>();

            var auth0Domain = config["Auth0:Authority"];
            if (string.IsNullOrEmpty(auth0Domain))
            {
                throw new ArgumentException("Auth0 domain is not configured.");
            }

            _authenticationApiClient = new AuthenticationApiClient(new Uri(auth0Domain));
            // TODO set up respawn for avoiding changes during tests https://github.com/jbogard/respawn
        }

        public async Task DisposeAsync()
        {
            foreach (var userId in _createdUserIds)
            {
                try
                {
                    await _managementApiClient.Users.DeleteAsync(userId);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to delete user with ID {userId}: {ex.Message}");
                }
            }

            _createdUserIds.Clear();
        }

        public async Task InitializeAsync()
        {
            foreach (UserRole role in Enum.GetValues(typeof(UserRole)))
            {
                await CreateUserWithRole(role);
            }
        }

        protected async Task LoginAsync(UserRole testLoginRole)
        {
            roleSessionStorage.TryGetValue(testLoginRole, out string? token);

            if (token is null)
            {
                var semaphore = roleSemaphores.GetOrAdd(testLoginRole, _ => new SemaphoreSlim(1, 1));

                await semaphore.WaitAsync();

                try
                {
                    var clientId = _factory.Configuration["Auth0:BlazorClientId"];
                    var clientSecret = _factory.Configuration["Auth0:BlazorClientSecret"];
                    var audience = _factory.Configuration["Auth0:Audience"];

                    var tokenRequest = new ResourceOwnerTokenRequest
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret,
                        Scope = "openid profile email",
                        Audience = audience,
                        Username = testLoginRole.GetEmail(),
                        Password = testLoginRole.GetPassword(),
                    };

                    token = await RunTaskWithRetries(async () => await SendLoginRequest(tokenRequest), (string? t) => t is null);

                    if (token is null) throw new Exception("Failed to fetch login token");
                }
                finally
                {
                    semaphore.Release();
                }
            }

            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        protected void LogOutAsync()
        {
            _client.DefaultRequestHeaders.Authorization = null;
        }

        private async Task<string?> SendLoginRequest(ResourceOwnerTokenRequest tokenRequest)
        {
            try
            {
                AccessTokenResponse? tokenResponse = await _authenticationApiClient.GetTokenAsync(tokenRequest);
                string token = tokenResponse.AccessToken;
                return token;
            }
            catch (RateLimitApiException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"Rate limit exceeded. Retrying in a little while...");
                //Delay so that auth0 api doesn't throw a rate limit exception
                return null;
            }
        }

        private async Task CreateUserWithRole(UserRole testLoginRole)
        {
            try
            {
                UserCreateRequest request = MakeUserCreateRequest(testLoginRole.GetId(), testLoginRole);
                await RunTaskWithRetries(async () => await SendCreateUserRequest(testLoginRole, request), (t) => !t);
            }
            catch (ErrorApiException)
            {
                // User already exists
                return;
            }
        }

        private static UserCreateRequest MakeUserCreateRequest(int testId, UserRole testLoginRole)
        {
            return MakeUserCreateRequest(testId, testLoginRole.GetUserName(), testLoginRole.GetEmail(), testLoginRole.GetPassword());
        }

        private static UserCreateRequest MakeUserCreateRequest(int testId, string username, string email, string password)
        {
            return new UserCreateRequest
            {
                UserName = username,
                Email = email,
                Connection = "Username-Password-Authentication",
                Password = password,
                AppMetadata = new Dictionary<string, object>
                        {
                            { "buutUserId", testId.ToString() },
                        }
            };
        }

        private async Task<bool> SendCreateUserRequest(UserRole testLoginRole, UserCreateRequest request)
        {
            try
            {
                var user = await _managementApiClient.Users.CreateAsync(request);

                _createdUserIds.Add(user.UserId);

                var roles = await _managementApiClient.Roles.GetAllAsync(new GetRolesRequest { NameFilter = testLoginRole.GetRole() });
                var role = roles.FirstOrDefault() ?? throw new Exception($"Role '{testLoginRole.GetRole()}' not found");

                await _managementApiClient.Users.AssignRolesAsync(user.UserId, new AssignRolesRequest
                {
                    Roles = [role.Id]
                });

                return true;
            }
            catch (RateLimitApiException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"Rate limit exceeded. Retrying in a little while...");
                //Delay so that auth0 api doesn't throw a rate limit exception
            }
            return false;
        }

        private static async Task<T?> RunTaskWithRetries<T>(Func<Task<T?>> callback, Predicate<T?> isSuccessful, int retryLimit = 10)
        {
            int retries = 0;
            T? result = default;
            while (retries <= retryLimit && isSuccessful(result))
            {
                result = await callback();
                // Exponential delay to avoid race conditions
                await Task.Delay(2 ^ retries++ * 1_000);
            }
            return result;
        }

        protected async Task TestForbiddenAccessForEndpoint(string url, UserRole testLoginRole, string httpMethod)
        {
            await LoginAsync(testLoginRole);

            var response = await GetResponseForRequest(url, httpMethod);

            response?.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }

        protected async Task TestUnauthorizedAccessForEndpoint(string url, string httpMethod)
        {
            var response = await GetResponseForRequest(url, httpMethod);

            response?.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }

        private async Task<HttpResponseMessage?> GetResponseForRequest(string url, string httpMethod) => httpMethod switch
        {
            "GET" => await _client.GetAsync(url),
            "POST" => await _client.PostAsJsonAsync(url, new object()),
            "PATCH" => await _client.PatchAsJsonAsync(url, new object()),
            _ => null,
        };

        private async Task<Auth0.ManagementApi.Models.User?> FindUserByBuutUserId(int buutUserId)
        {
            var users = await _managementApiClient.Users.GetAllAsync(new GetUsersRequest() { Query = $"app_metadata.buutUserId:{buutUserId}" });
            return users.FirstOrDefault();
        }

        public async Task DeleteAuth0UserByBuutUserId(int buutUserId)
        {
            try
            {
                await RunTaskWithRetries(async () => await SendDeleteAuth0UserRequest(buutUserId), (t) => !t, 15);
            }
            catch (Exception ex)
            {
                throw new Exception("Delete request failed.", ex);
            }
        }

        private async Task<bool> SendDeleteAuth0UserRequest(int buutUserId)
        {
            try
            {
                var user = await FindUserByBuutUserId(buutUserId);
                if (user != null)
                {
                    await _managementApiClient.Users.DeleteAsync(user.UserId);
                    return true;
                }
            }
            catch (RateLimitApiException ex)
            {
                Console.WriteLine(ex.ToString());
                Console.WriteLine($"Delete Rate limit exceeded. Retrying in a little bit...");
                //Delay so that auth0 api doesn't throw a rate limit exception
            }

            return false;
        }

        public async Task RegisterValidAuth0User()
        {
            await RunTaskWithRetries(RegisterValidUser, (u) => !u);
        }

        private async Task<bool> RegisterValidUser()
        {
            try
            {
                const int buutUserId = 6;
                const string email = "user3@example.com";
                const string password = "SecureP@ssw0rd123!";
                const UserRole role = UserRole.Guest;

                UserCreateRequest request = MakeUserCreateRequest(buutUserId, email, email, password);

                return await SendCreateUserRequest(role, request);
            }
            catch (RateLimitApiException ex)
            {
                Console.WriteLine($"Rate limit exceeded: {ex.Message}. Retrying...");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to create user: {ex.Message}");
            }

            return false;
        }



    }
}
