using Rise.Server.Tests.Fixtures;
using Shouldly;
using System.Net.Http.Json;
using System.Net;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Rise.Shared.Users;

namespace Rise.Server.Tests.Controllers
{
    public class ReservationControllerTest(ApiWebApplicationFactory fixture) : IntegrationTest(fixture, "Reservation")
    {
        [Theory]
        [InlineData("me", UserRole.Guest, "GET")]
        [InlineData("", UserRole.Guest, "POST")]
        [InlineData("1", UserRole.Guest, "GET")]
        public async Task Call_ReservationController_Endpoints_ExpectForbidden(string url, UserRole testLoginRole, string httpMethod)
        {
            await TestForbiddenAccessForEndpoint(url, testLoginRole, httpMethod);
        }

        [Theory]
        [InlineData("me", "GET")]
        [InlineData("", "POST")]
        [InlineData("1", "GET")]
        public async Task Call_ReservationController_Endpoints_ExpectUnauthorized(string url, string httpMethod)
        {
            await TestUnauthorizedAccessForEndpoint(url, httpMethod);
        }

        [Fact]
        public async Task GET_CurrentUser_UpcomingReservations_WithNoParameters_FirstPage_ExpectOk_5OrLessReservations()
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync("me");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationsPage = await response.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            reservationsPage.ShouldNotBeNull();
            reservationsPage.Data.ShouldNotBeEmpty();
            reservationsPage.Data.Count().ShouldBeLessThanOrEqualTo(5);
            reservationsPage.IsFirstPage.ShouldBeTrue();
            reservationsPage.PreviousId.ShouldBeNull();
            reservationsPage.NextId.ShouldNotBeNull();
            reservationsPage.Data.ShouldAllBe(r => r.Date >= DateOnly.FromDateTime(DateTime.Now));
        }

        // baken zelf de range af van de reservations van een maand geleden + 5
        [Fact]
        public async Task GET_CurrentUser_PastReservations_WithNoParameters_FirstPage_ExpectOk_5OrLessReservations()
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync("me?getPast=true");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationsPage = await response.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            reservationsPage.ShouldNotBeNull();
            reservationsPage.Data.ShouldNotBeEmpty();
            reservationsPage.Data.Count().ShouldBeLessThanOrEqualTo(5);
            reservationsPage.IsFirstPage.ShouldBeTrue();
            reservationsPage.PreviousId.ShouldBeNull();
            reservationsPage.NextId.ShouldNotBeNull();

            // Additional checks for past reservations
            var today = DateOnly.FromDateTime(DateTime.Today);
            var oneMonthAgo = today.AddMonths(-1);

            // Verify all returned reservations are from the past
            reservationsPage.Data.ShouldAllBe(r => r.Date < today);
            // start van 2 dagen geleden tot in het verleden
            reservationsPage.Data.ShouldAllBe(r => r.Date <= today.AddDays(-2));
        }

        [Fact]
        public async Task GET_CurrentUser_PastReservations_NextPage_ExpectOk_5OrLessReservations()
        {
            await LoginAsync(UserRole.Member);
            // Get first page
            var firstResponse = await _client.GetAsync("me?getPast=true");
            firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);
            var firstPage = await firstResponse.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            firstPage.ShouldNotBeNull();
            firstPage.NextId.ShouldNotBeNull();

            // Store the last ID from first page to verify cursor implementation
            var lastIdFromFirstPage = firstPage.Data.Last().Id;

            // Get next page using cursor
            var nextResponse = await _client.GetAsync($"me?getPast=true&cursor={firstPage.NextId}&isNextPage=true");
            nextResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            var nextPage = await nextResponse.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            nextPage.ShouldNotBeNull();

            // de eerste van de lijst moet de cursor zijn van de vorige pagina
            nextPage.PreviousId.ShouldNotBe(lastIdFromFirstPage);  // Previous cursor should point to last item of first page
            nextPage.Data.First().Id.ShouldBeLessThan(lastIdFromFirstPage);  // Items should be ordered by ID descending

            var today = DateOnly.FromDateTime(DateTime.Now);
            var oneMonthAgo = today.AddMonths(-1);
            nextPage.Data.ShouldNotBeEmpty();
            nextPage.Data.Count().ShouldBeLessThanOrEqualTo(5);
            nextPage.IsFirstPage.ShouldBeFalse();
            nextPage.PreviousId.ShouldNotBeNull();
            nextPage.Data.ShouldAllBe(r => r.Date < DateOnly.FromDateTime(DateTime.Now));

            nextPage.Data.First().Id.ShouldNotBe(firstPage.Data.First().Id);
            // van 8 dagen geleden tot 13 dagen geleden
            // moet de cursor meegeven van de pagina
            DateOnly start = today.AddDays(-15);
            DateOnly end = today.AddDays(-9);

            nextPage.Data.ShouldAllBe(r => r.Date < DateOnly.FromDateTime(DateTime.Now));

        }

        [Theory]
        [InlineData(7)]
        [InlineData(15)]
        public async Task GET_CurrentUser_UpcomingReservations_WithVaryingPageSize_ExpectOk_PageSizeAmountOrLessReservations(int pageSize)
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationsPage = await response.Content.ReadFromJsonAsync<ItemsPageDto<ReservationDto>>();
            reservationsPage.ShouldNotBeNull();
            reservationsPage.Data.ShouldNotBeEmpty();
            reservationsPage.Data.Count().ShouldBeLessThanOrEqualTo(pageSize);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(0)]
        public async Task GET_CurrentUser_Reservations_WithInvalidPageSize_ExpectBadRequest(int pageSize)
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("invalid")]
        public async Task GET_CurrentUser_Reservations_WithInvalidPageSizeType_ExpectBadRequest(string pageSize)
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync($"me?pageSize={pageSize}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(-56)]
        public async Task GET_CurrentUser_Reservations_WithInvalidNumber_ExpectBadRequest(int cursor)
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync($"me?cursor={cursor}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData("invalid")]
        public async Task GET_CurrentUser_Reservations_WithInvalidType_ExpectBadRequest(string cursor)
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync($"me?cursor={cursor}");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_CurrentUser_Reservations_NextPage_WithValidCursor_IsNextPageNull_ExpectBadRequest()
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync($"me?cursor=17");
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }


        [Fact]
        public async Task POST_CreateReservation_WithValidTimeSlot_ExpectCreated()
        {
            await LoginAsync(UserRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = 100
            };


            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.Created);
            var reservationId = await response.Content.ReadFromJsonAsync<int>();
            reservationId.ShouldBeGreaterThan(0);
        }


        [Fact]
        public async Task POST_CreateReservation_WithDuplicateTimeSlot_ExpectConflict()
        {
            await LoginAsync(UserRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = 33
            };

            //first a reservation should be created then it shouldn't be for the same user
            var response1 = await _client.PostAsJsonAsync("", request);
            response1.StatusCode.ShouldBe(HttpStatusCode.Created);
            var response2 = await _client.PostAsJsonAsync("", request);
            response2.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }


        [Fact]
        public async Task POST_CreateReservation_WithQueryParameters_ExpectBadRequest()
        {
            await LoginAsync(UserRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = 1
            };

            var response = await _client.PostAsJsonAsync("?badrequest=true", request);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_CreateReservation_WithNoAvailableBoats_ExpectConflict()
        {
            await LoginAsync(UserRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = 1
            };

            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task POST_CreateReservation_WithInvalidTimeSlot_ExpectNotFound()
        {
            await LoginAsync(UserRole.Member);

            var request = new CreateReservationDto
            {
                TimeSlotId = -1
            };


            var response = await _client.PostAsJsonAsync("", request);
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task POST_CreateReservation_WithMissingTimeSlotId_ExpectBadRequest()
        {
            await LoginAsync(UserRole.Member);

            var request = new CreateReservationDto();

            var response = await _client.PostAsJsonAsync("", request);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task GET_ReservationDetails_WithExistingId_ExpectOk()
        {
            await LoginAsync(UserRole.Member);

            var existingId = 1;
            var response = await _client.GetAsync($"{existingId}");


            response.StatusCode.ShouldBe(HttpStatusCode.OK);


            var reservationDetails = await response.Content.ReadFromJsonAsync<ReservationDetailsDto>();
            reservationDetails.ShouldNotBeNull();
            reservationDetails.Id.ShouldBe(existingId);
        }

        [Fact]
        public async Task GET_ReservationDetails_WithNonExistentId_ExpectNotFound()
        {
            await LoginAsync(UserRole.Member);

            var nonExistentId = 9999;
            var response = await _client.GetAsync($"{nonExistentId}");

            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("abc")]
        [InlineData("@!#")]
        public async Task GET_ReservationDetails_WithInvalidId_ExpectBadRequest(string invalidId)
        {
            await LoginAsync(UserRole.Member);

            var response = await _client.GetAsync($"{invalidId}");

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PATCH_CancelReservation_WithValidId_ExpectOk()
        {
            await LoginAsync(UserRole.Member);

            var validReservationId = 79;


            var response = await _client.PatchAsync($"cancel/{validReservationId}", null);
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var reservationDetailsResponse = await _client.GetAsync($"{validReservationId}");
            reservationDetailsResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            var reservationDetails = await reservationDetailsResponse.Content.ReadFromJsonAsync<ReservationDetailsDto>();
            reservationDetails.ShouldNotBeNull();
            reservationDetails.Id.ShouldBe(validReservationId);
            reservationDetails.IsDeleted.ShouldBeTrue();
        }


        [Fact]
        public async Task PATCH_CancelReservation_WithNonExistentId_ExpectNotFound()
        {
            await LoginAsync(UserRole.Member);
            var nonExistentReservationId = 9999;


            var response = await _client.PatchAsync($"cancel/{nonExistentReservationId}", null);


            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
        [Fact]
        public async Task PATCH_CancelReservation_AlreadyCancelledReservation_ExpectBadRequest()
        {
            await LoginAsync(UserRole.Member);
            var cancelledReservationId = 50;
            await _client.PatchAsync($"cancel/{cancelledReservationId}", null);

            var response = await _client.PatchAsync($"cancel/{cancelledReservationId}", null);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task PATCH_CancelReservation_WithinTwoDaysOfReservation_ExpectBadRequest()
        {
            await LoginAsync(UserRole.Member);
            var reservationIdWithinTwoDays = 1;


            var response = await _client.PatchAsync($"cancel/{reservationIdWithinTwoDays}", null);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }
        [Fact]
        public async Task PATCH_CancelReservation_ByAdminWithinTwoDays_ExpectSuccess()
        {
            await LoginAsync(UserRole.Administrator);
            var reservationIdWithinTwoDays = 59;

            var response = await _client.PatchAsync($"cancel/{reservationIdWithinTwoDays}", null);

            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        }
        [Fact]
        public async Task PATCH_CancelReservation_ByAdminOnSameDay_ExpectSuccess()
        {
            await LoginAsync(UserRole.Administrator);
            var reservationIdOnSameDay = 56;

            var response = await _client.PatchAsync($"cancel/{reservationIdOnSameDay}", null);

            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        }

        [Fact]
        public async Task PATCH_CancelReservation_ByUserForPastReservation_ExpectBadRequest()
        {
            await LoginAsync(UserRole.Member);
            var pastReservationId = 5;

            var response = await _client.PatchAsync($"cancel/{pastReservationId}", null);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        }
        [Fact]
        public async Task PATCH_CancelReservation_ByAdminForPastReservation_ExpectBadRequest()
        {
            await LoginAsync(UserRole.Administrator);
            var pastReservationId = 5;

            var response = await _client.PatchAsync($"cancel/{pastReservationId}", null);

            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);

        }

    }
}
