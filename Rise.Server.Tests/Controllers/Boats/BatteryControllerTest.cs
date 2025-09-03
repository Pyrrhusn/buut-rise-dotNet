using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Rise.Server.Tests.Fixtures;
using Rise.Shared.Boats;
using Rise.Shared.Users;
using Shouldly;

namespace Rise.Server.Tests.Controllers.Boats
{
    public class BatteryControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "Battery")
    {
        private const string universalDateFormat = "yyyy-MM-dd";
        private const string defaultValidationErrorTitle = "One or more validation errors occurred.";
        private const string validBatteryType = "lithium";
        private const int validMentorId = 2;
        private const int validBatteryId = 1;


        [Theory]
        [InlineData("1", "GET")]
        [InlineData("1", "PUT")]
        public async Task Call_TimeSlotController_Endpoints_ExpectUnauthorized(string url, string httpMethod)
        {
            await TestUnauthorizedAccessForEndpoint(url, httpMethod);
        }

        [Fact]
        public async Task Get_NotExistingBattery_NotFound()
        {
            await LoginAsync(UserRole.Administrator);
            var response = await _client.GetAsync(int.MaxValue.ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);


        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        public async Task Get_InvalidBatteryId_BadRequest(int? batteryId)
        {
            await LoginAsync(UserRole.Administrator);
            var response = await _client.GetAsync(batteryId.ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            problemDetails?.Errors["id"][0].ShouldBe("Battery id must be positive");


        }

        [Fact]
        public async Task Get_ExistingBattery_Found()
        {
            await LoginAsync(UserRole.Administrator);
            int batteryId = 2;
            var response = await _client.GetAsync(batteryId.ToString());

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var details = await response.Content.ReadFromJsonAsync<BatteryDto>();
            details.ShouldNotBeNull();

            details.Id.ShouldBe(batteryId);
            details.Type.ShouldBe("Loodzuur");
            details.Mentor.Id.ShouldBe(2);
            details.Mentor.FullName.ShouldBe("de Clerk, Bram");
            details.Mentor.FirstName.ShouldBe("Bram");
            details.Mentor.FamilyName.ShouldBe("de Clerk");

        }

        [Fact]
        public async Task PUT_NotExistingBattery_NotFound()
        {
            await LoginAsync(UserRole.Administrator);
            var response = await _client.PutAsJsonAsync(int.MaxValue.ToString(), new BatteryUpdateDto() { Type = validBatteryType, MentorId = validMentorId });

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        public async Task PUT_InvalidBatteryId_BadRequest(int? batteryId)
        {
            await LoginAsync(UserRole.Administrator);
            var response = await _client.PutAsJsonAsync(batteryId.ToString(), new BatteryUpdateDto() { Type = validBatteryType, MentorId = validMentorId });

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            problemDetails?.Errors["id"][0].ShouldBe("Battery id must be positive");


        }

        [Fact]
        public async Task PUT_NotFoundMentor_NotFound()
        {
            await LoginAsync(UserRole.Administrator);
            var response = await _client.PutAsJsonAsync(validBatteryId.ToString(), new BatteryUpdateDto() { Type = validBatteryType, MentorId = int.MaxValue });

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);


        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        public async Task PUT_InvalidMentorId_BadRequest(int mentorId)
        {
            await LoginAsync(UserRole.Administrator);
            var response = await _client.PutAsJsonAsync(validBatteryId.ToString(), new BatteryUpdateDto() { Type = validBatteryType, MentorId = mentorId });

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var problemDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();

            problemDetails?.Errors["MentorId"][0].ShouldBe("'Mentor Id' must be greater than '0'.");


        }

        [Fact]
        public async Task PUT_ValidBattery_UpdateBattery()
        {
            await LoginAsync(UserRole.Administrator);
            var response = await _client.PutAsJsonAsync(validBatteryId.ToString(), new BatteryUpdateDto() { Type = validBatteryType, MentorId = validMentorId });

            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            BatteryDto newBattery = (await response.Content.ReadFromJsonAsync<BatteryDto>())!;

            newBattery.Type.ShouldBe(validBatteryType);
            newBattery.Mentor.Id.ShouldBe(validMentorId);
            newBattery.Mentor.FullName.ShouldBe("de Clerk, Bram");
            newBattery.Mentor.FirstName.ShouldBe("Bram");
            newBattery.Mentor.FamilyName.ShouldBe("de Clerk");

        }
    }
}