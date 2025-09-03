using Rise.Server.Tests.Fixtures;
using Shouldly;
using Rise.Shared.TimeSlots;
using System.Net.Http.Json;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Rise.Shared.Users;

namespace Rise.Server.Tests.Controllers.TimeSlots
{
    public class TimeSlotControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "TimeSlot")
    {
        private const string universalDateFormat = "yyyy-MM-dd";
        private const string defaultValidationErrorTitle = "One or more validation errors occurred.";

        private static string DateOnlyToUniversalDate(DateOnly date)
        {
            return date.ToString(universalDateFormat);
        }
        private static string MakeTimeSlotRangeUrl(DateOnly? startDate, DateOnly? endDate)
        {
            List<string> queries = [];
            if (startDate is not null) queries.Add($"startDate={DateOnlyToUniversalDate((DateOnly)startDate)}");
            if (endDate is not null) queries.Add($"endDate={DateOnlyToUniversalDate((DateOnly)endDate)}");

            string joinedQueries = queries.Count == 0 ? "" : ("?" + string.Join("&", queries));

            return $"range{joinedQueries}";
        }

        [Theory]
        [InlineData("range", "GET")]
        [InlineData("2024/11/15", "GET")]
        public async Task Call_TimeSlotController_Endpoints_ExpectUnauthorized(string url, string httpMethod)
        {
            await TestUnauthorizedAccessForEndpoint(url, httpMethod);
        }

        [Fact]
        public async Task GET_ValidDateRange_GivesDates()
        {
            await LoginAsync(UserRole.Guest);

            int daysDifference = 7;
            DateOnly startDate = DateOnly.FromDateTime(DateTime.Now);
            DateOnly endDate = startDate.AddDays(daysDifference);
            TimeSlotRangeInfoDto response = (await _client.GetFromJsonAsync<TimeSlotRangeInfoDto>(MakeTimeSlotRangeUrl(startDate, endDate)))!;
            response.TotalDays.ShouldBe(8);
            response.Days.ShouldBe([
                new(startDate, false, false, true),
                new(startDate.AddDays(1), false, false, true),
                new(startDate.AddDays(2), false, false, true),
                new(startDate.AddDays(3), false, true, false),
                new(startDate.AddDays(4), false, false, false),
                new(startDate.AddDays(5), false, true, true),
                new(startDate.AddDays(6), false, true, true),
                new(startDate.AddDays(7), false, true, true),
            ]);

        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task GET_NoDateRange_GetsDefaultDate(bool startDate, bool endDate)
        {
            await LoginAsync(UserRole.Guest);

            DateOnly defaultDay = DateOnly.MinValue;
            string uri = MakeTimeSlotRangeUrl(startDate ? defaultDay : null, endDate ? defaultDay : null);
            TimeSlotRangeInfoDto response = (await _client.GetFromJsonAsync<TimeSlotRangeInfoDto>(uri))!;
            response.TotalDays.ShouldBe(1);
            response.Days.ShouldBe([
                new(defaultDay, false, false),
            ]);

        }

        [Fact]
        public async Task GET_InvalidStartDate_Expects404()
        {
            await LoginAsync(UserRole.Guest);

            var response = await _client.GetAsync($"range?startDate=&endDate={DateOnlyToUniversalDate(DateOnly.MaxValue)}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("startDate");

        }

        [Fact]
        public async Task GET_InvalidEndDate_Expects404()
        {
            await LoginAsync(UserRole.Guest);

            var response = await _client.GetAsync($"range?startDate={DateOnlyToUniversalDate(DateOnly.MinValue)}&endDate=");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("endDate");

        }

        [Fact]
        public async Task GET_EndDateBeforeStartDate_Expects404()
        {
            await LoginAsync(UserRole.Guest);

            var response = await _client.GetAsync(MakeTimeSlotRangeUrl(DateOnly.MaxValue, DateOnly.MinValue));
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

            var result = (await response.Content.ReadFromJsonAsync<ValidationProblemDetails>())!;

            result.Title.ShouldBe(defaultValidationErrorTitle);
            result.Errors.Count.ShouldBe(1);
            result.Errors.ShouldContainKey("DateRange");

        }

        [Fact]
        public async Task GET_TimeSlotsByDate_GivesTimeSlots()
        {
            await LoginAsync(UserRole.Guest);

            DateTime threeDaysInAdvance = DateTime.Now.AddDays(3);
            int year = threeDaysInAdvance.Year;
            int month = threeDaysInAdvance.Month;
            int day = threeDaysInAdvance.Day;


            List<TimeSlotDto> response = (await _client.GetFromJsonAsync<List<TimeSlotDto>>($"{year}/{month}/{day}"))!;

            // Assert
            response.ShouldNotBeEmpty();
            response.Count.ShouldBe(3);

            response.ShouldContain(ts => ts.Start.Equals(TimeOnly.Parse("10:00:00")) && ts.End.Equals(TimeOnly.Parse("11:30:00")));
            response.ShouldContain(ts => ts.Start.Equals(TimeOnly.Parse("13:00:00")) && ts.End.Equals(TimeOnly.Parse("14:00:00")));
            response.ShouldContain(ts => ts.Start.Equals(TimeOnly.Parse("16:30:00")) && ts.End.Equals(TimeOnly.Parse("18:45:00")));

        }



        [Theory]
        [InlineData(8)]
        public async Task GET_TimeSlotsByDate_GivesNoTimeSlots(int daysFromNow)
        {
            await LoginAsync(UserRole.Guest);

            DateTime daysInAdvance = DateTime.Now.AddDays(daysFromNow);
            int year = daysInAdvance.Year;
            int month = daysInAdvance.Month;
            int day = daysInAdvance.Day;


            List<TimeSlotDto> response = (await _client.GetFromJsonAsync<List<TimeSlotDto>>($"{year}/{month}/{day}"))!;

            // Assert
            response.ShouldBeEmpty();
            response.Count.ShouldBe(0);

        }

        [Fact]
        public async Task GET_TimeSlotsByDate_InvalidDate_ReturnsBadRequest()
        {
            await LoginAsync(UserRole.Guest);

            DateTime tomorrow = DateTime.Now.AddDays(1);
            int year = tomorrow.Year;
            int month = 13;
            int day = tomorrow.Day;


            var response = await _client.GetAsync($"{year}/{month}/{day}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        }

        [Fact]
        public async Task CreateTimeSlot_ValidData_CreatesSuccessfully()
        {
            await LoginAsync(UserRole.Administrator);
            var dto = new CreateTimeSlotDto
            {
                CruisePeriodId = 1,
                TimeSlots = new List<TimeSlotRange>
                {
                    new() { Start = new TimeOnly(22, 0), End = new TimeOnly(23, 40) },
                }
            };

            var response = await _client.PostAsJsonAsync("", dto);

            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var timeSlotsCount = await response.Content.ReadFromJsonAsync<int>();
            timeSlotsCount.ShouldBe(90);
        }

        [Fact]
        public async Task CreateTimeSlot_UnauthorizedUser_ReturnsForbidden()
        {
            await LoginAsync(UserRole.Guest);
            var dto = new CreateTimeSlotDto
            {
                CruisePeriodId = 1,
                TimeSlots = new List<TimeSlotRange>
                {
                    new() { Start = new TimeOnly(10, 0), End = new TimeOnly(11, 30) }
                }
            };

            var response = await _client.PostAsJsonAsync("", dto);

            response.StatusCode.ShouldBe(HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task CreateTimeSlot_InvalidCruisePeriod_ReturnsNotFound()
        {
            await LoginAsync(UserRole.Administrator);
            var dto = new CreateTimeSlotDto
            {
                CruisePeriodId = 99999, // Non-existent ID
                TimeSlots = new List<TimeSlotRange>
                {
                    new() { Start = new TimeOnly(10, 0), End = new TimeOnly(11, 30) }
                }
            };

            var response = await _client.PostAsJsonAsync("", dto);

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task CreateTimeSlot_InvalidCruisePeriodId_ReturnsBadRequest(int invalidCruisePeriodId)
        {
            await LoginAsync(UserRole.Administrator);
            var dto = new CreateTimeSlotDto
            {
                CruisePeriodId = invalidCruisePeriodId,
                TimeSlots = new List<TimeSlotRange>
                {
                    new() { Start = new TimeOnly(10, 0), End = new TimeOnly(11, 30) }
                }
            };

            var response = await _client.PostAsJsonAsync("", dto);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateTimeSlot_EmptyTimeSlots_ReturnsBadRequest()
        {
            await LoginAsync(UserRole.Administrator);
            var dto = new CreateTimeSlotDto
            {
                CruisePeriodId = 1,
                TimeSlots = new List<TimeSlotRange>()
            };

            var response = await _client.PostAsJsonAsync("", dto);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateTimeSlot_InvalidDuration_ReturnsBadRequest()
        {
            await LoginAsync(UserRole.Administrator);
            var dto = new CreateTimeSlotDto
            {
                CruisePeriodId = 1,
                TimeSlots = new List<TimeSlotRange>
                {
                    new() { Start = new TimeOnly(10, 0), End = new TimeOnly(11, 0) } // Less than 1.5 hours
                }
            };

            var response = await _client.PostAsJsonAsync("", dto);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateTimeSlot_OverlappingTimeSlots_ReturnsBadRequest()
        {
            await LoginAsync(UserRole.Administrator);
            var dto = new CreateTimeSlotDto
            {
                CruisePeriodId = 1,
                TimeSlots = new List<TimeSlotRange>
                {
                    new() { Start = new TimeOnly(10, 0), End = new TimeOnly(11, 30) },
                    new() { Start = new TimeOnly(11, 0), End = new TimeOnly(12, 30) } // Overlaps with previous slot
                }
            };

            var response = await _client.PostAsJsonAsync("", dto);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateTimeSlot_EndTimeBeforeStartTime_ReturnsBadRequest()
        {
            await LoginAsync(UserRole.Administrator);
            var dto = new CreateTimeSlotDto
            {
                CruisePeriodId = 1,
                TimeSlots = new List<TimeSlotRange>
                {
                    new() { Start = new TimeOnly(11, 0), End = new TimeOnly(10, 0) }
                }
            };

            var response = await _client.PostAsJsonAsync("", dto);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }


    }
}