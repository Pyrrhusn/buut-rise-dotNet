using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.Users;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Rise.Server.Tests.Controllers.Users
{
    public class UserControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "User")
    {
        [Theory]
        [InlineData("users?role=Guest", UserRole.Guest)]
        [InlineData("users?role=Member", UserRole.Guest)]
        [InlineData("users?role=Administrator", UserRole.Member)]
        [InlineData("users?role=Guest", UserRole.Member)]
        public async Task GetUsersByRole_WithNonAdminRole_ReturnsForbidden(string url, UserRole testLoginRole)
        {
            await LoginAsync(testLoginRole);
            await Task.Delay(TimeSpan.FromSeconds(2));
            var response = await _client.GetAsync(url);
            response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }

        [Theory]
        [InlineData("users?role=Guest")]
        [InlineData("users?role=Member")]
        [InlineData("1")]
        public async Task UserEndpoints_WithoutAuthentication_ReturnsUnauthorized(string url)
        {
            var response = await _client.GetAsync(url);
            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);
        }


        [Theory]
        [InlineData(0, 10)]
        [InlineData(1, 0)]
        [InlineData(-1, 5)]
        [InlineData(1, -5)]
        public async Task GetUsersByRole_WithInvalidPagination_ReturnsBadRequest(int page, int pageSize)
        {
            await LoginAsync(UserRole.Administrator);


            var response = await _client.GetAsync($"users?role={UserRole.Guest}&page={page}&pageSize={pageSize}");

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GetUserDetails_WithValidId_ReturnsCorrectUser()
        {
            const int userId = 1;
            await LoginAsync(UserRole.Administrator);


            var response = await _client.GetFromJsonAsync<UserDetailDto>($"{userId}");

            response.ShouldNotBeNull();
            response.Id.ShouldBe(userId);
            response.Email.ShouldNotBeNullOrEmpty();
            response.FirstName.ShouldNotBeNullOrEmpty();
            response.FamilyName.ShouldNotBeNullOrEmpty();
            response.PhoneNumber.ShouldNotBeNullOrEmpty();
            response.Address.ShouldNotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(99999)]
        public async Task GetUserDetails_WithInvalidId_ReturnsNotFound(int userId)
        {
            await LoginAsync(UserRole.Administrator);

            var response = await _client.GetAsync($"{userId}");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        // TODO bring back AddMemberRole_AsAdmin_SuccessfullyAddsRole, this is a temporary desperate measure
        // [Fact]
        // public async Task AddMemberRole_AsAdmin_SuccessfullyAddsRole()
        // {
        //     await LoginAsync(UserRole.Administrator);

        //     //create a valid guest user to add member role to
        //     await RegisterValidAuth0User();

        //     //validuser has id 6
        //     var request = new AddMemberRoleDto { UserId = 6, Role = UserRole.Member };

        //     await Task.Delay(TimeSpan.FromSeconds(2));

        //     var response = await _client.PostAsJsonAsync("role", request);

        //     response.StatusCode.ShouldBe(HttpStatusCode.OK);

        //     //delete the user we created in auth0
        //     await DeleteAuth0UserByBuutUserId(6);

        // }

        [Theory]
        [InlineData(UserRole.Administrator)]
        [InlineData(UserRole.Guest)]
        public async Task AddMemberRole_WithNonMemberRole_ReturnsBadRequest(UserRole roleToAdd)
        {
            // Arrange
            await LoginAsync(UserRole.Administrator);
            var request = new AddMemberRoleDto { UserId = 1, Role = roleToAdd };

            // Act
            var response = await _client.PostAsJsonAsync("role", request);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [ClassData(typeof(RegisterUserDtoValidData))]
        public async Task POST_RegisterUser_ReturnsUserId(UserRegistrationModelDto userDto)
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            var response = await _client.PostAsJsonAsync("register", userDto);

            response.ShouldNotBeNull();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var userId = await response.Content.ReadFromJsonAsync<int>();
            userId.ShouldBeGreaterThan(0);

            await DeleteAuth0UserByBuutUserId(userId);
        }

        [Theory]
        [ClassData(typeof(RegisterUserDtoBadData))]
        public async Task POST_RegisterUser_ReturnsBadRequest(UserRegistrationModelDto userDto)
        {
            var response = await _client.PostAsJsonAsync("register", userDto);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_RegisterUser_RegisterExistingUser_ReturnConflict()
        {
            await Task.Delay(TimeSpan.FromSeconds(2));

            var userDto = new UserRegistrationModelDto()
            {
                Email = "rand.om@example.com",
                Password = "SecureP@ssw0rd123",
                FirstName = "John",
                FamilyName = "Doe",
                PhoneNumber = "+32471123456",
                DateOfBirth = new DateTime(2005, 1, 1),
                Address = new()
                {
                    Street = "Fabiolalaan",
                    Number = "10",
                    City = "Gent",
                    PostalCode = "9000",
                    Country = "Belgium"
                }
            };

            var succesfulResponse = await _client.PostAsJsonAsync("register", userDto);
            var userId = await succesfulResponse.Content.ReadFromJsonAsync<int>();

            var failedResponse = await _client.PostAsJsonAsync("register", userDto);
            failedResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);

            await DeleteAuth0UserByBuutUserId(userId);
        }

        [Fact]
        public async Task GetUsersByFullName_NotLoggedIn_Unauthorized()
        {
            LogOutAsync();
            var response = await _client.GetAsync("names");

            response.StatusCode.ShouldBe(HttpStatusCode.Unauthorized);

        }


        [Theory]
        [InlineData(UserRole.Guest)]
        [InlineData(UserRole.Member)]
        public async Task GetUsersByFullName_NotAdmin_Forbidden(UserRole role)
        {
            LogOutAsync();
            await LoginAsync(role);
            var response = await _client.GetAsync("names");

            response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);

        }

        [Fact]
        public async Task GetUsersByFullName_NoGivenFilterName_OK()
        {
            await LoginAsync(UserRole.Administrator);

            int pageSize = 5;

            // First Page
            Dictionary<string, string?> queriesFirst = new()
            {
                ["pageSize"] = pageSize.ToString(),
            };

            string queryStringFirst = QueryHelpers.AddQueryString("names", queriesFirst);

            var responseFirstPage = await _client.GetAsync(queryStringFirst);

            responseFirstPage.StatusCode.ShouldBe(HttpStatusCode.OK);

            var response = await responseFirstPage.Content.ReadFromJsonAsync<IEnumerable<UserNameDto>>();
            response.ShouldNotBeNull();

            List<UserNameDto> users = response.ToList();
            users.Count.ShouldBe(9);
            users[0].FullName.ShouldBe("Barabich, Bas");
            users[1].FullName.ShouldBe("Chin, Bindo");
            users[2].FullName.ShouldBe("De Clerck, Kimberlie");
            users[3].FullName.ShouldBe("de Clerk, Bram");
            users[4].FullName.ShouldBe("Helks, Pushwant");
            users[5].FullName.ShouldBe("Her De Gaver, Patrick");
            users[6].FullName.ShouldBe("Montu, Sujan");
            users[7].FullName.ShouldBe("Piatti, Simon");
            users[8].FullName.ShouldBe("Serket, Xan");

            LogOutAsync();
        }

        [Fact]
        public async Task GetUsersByFullName_GivenFilterName_OK()
        {
            await LoginAsync(UserRole.Administrator);

            Dictionary<string, string?> queriesFirst = new()
            {
                ["partialName"] = "er",
            };

            string queryStringFirst = QueryHelpers.AddQueryString("names", queriesFirst);

            var responseFirstPage = await _client.GetAsync(queryStringFirst);

            responseFirstPage.StatusCode.ShouldBe(HttpStatusCode.OK);

            var userNames = await responseFirstPage.Content.ReadFromJsonAsync<IEnumerable<UserNameDto>>();
            userNames.ShouldNotBeNull();

            List<UserNameDto> usersFirst = userNames.ToList();
            usersFirst.Count.ShouldBe(4);
            usersFirst[0].FullName.ShouldBe("De Clerck, Kimberlie");
            usersFirst[1].FullName.ShouldBe("de Clerk, Bram");
            usersFirst[2].FullName.ShouldBe("Her De Gaver, Patrick");
            usersFirst[3].FullName.ShouldBe("Serket, Xan");

            LogOutAsync();
        }
    }
}