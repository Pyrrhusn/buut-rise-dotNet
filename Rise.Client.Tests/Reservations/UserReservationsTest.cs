using Microsoft.Playwright;
using Rise.Shared.Reservations;
using Rise.Shared.Pagination;
using System.Text.RegularExpressions;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Reservations
{
    public class UserReservationsTest : CustomAuthenticatedPageTest
    {
        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Member);
        }

        private const string UserReservationsUrl = "/reservations?CurrentTab=reservations";


        private readonly ReservationDto ValidReservation = new()
        {
            BoatId = 1,
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(5)),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatPersonalName = "Limba"
        };

        private readonly ReservationDto PastReservation = new()
        {
            BoatId = 2,
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-5)),
            Start = TimeOnly.Parse("09:00"),
            End = TimeOnly.Parse("12:00"),
            BoatPersonalName = "Speedy"
        };

        private async Task MockReservationsApi(int delayMs = 0)
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                if (delayMs > 0)
                    await Task.Delay(delayMs);

                var response = new ItemsPageDto<ReservationDto>()
                {
                    Data = [ValidReservation],
                    NextId = 1,
                    PreviousId = 1,
                    IsFirstPage = true
                };

                await route.FulfillAsync(new()
                {
                    ContentType = "application/json",
                    Body = System.Text.Json.JsonSerializer.Serialize(response)
                });
            });
        }
        
        private async Task MockPastReservationsApi()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                // await Task.Delay(3000);

                var response = new ItemsPageDto<ReservationDto>()
                {
                    Data = [PastReservation],
                    NextId = 1,
                    PreviousId = 1,
                    IsFirstPage = true
                };

                await route.FulfillAsync(new()
                {
                    ContentType = "application/json",
                    Body = System.Text.Json.JsonSerializer.Serialize(response)
                });
            });
        }

        private async Task MockEmptyReservationsApi()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                // await Task.Delay(3000);
                var response = new ItemsPageDto<ReservationDto>()
                {
                    Data = [],
                    NextId = null,
                    PreviousId = null,
                    IsFirstPage = true
                };
                await route.FulfillAsync(new()
                {
                    ContentType = "application/json",
                    Body = System.Text.Json.JsonSerializer.Serialize(response)
                });
            });
        }

        [Test]
        public async Task RedirectsToReservationDetails_WhenViewDetailsButtonClicked()
        {

            await MockReservationsApi();
            await NavigateToUrl(UserReservationsUrl);


            await Page.WaitForSelectorAsync("[data-testid='reservation-item']");
            await Expect(Page.GetByTestId("reservation-item")).ToBeVisibleAsync();
            var viewDetailsButton = Page.GetByTestId("view-reservation-details-button");
            await viewDetailsButton.ClickAsync();


            await Expect(Page).ToHaveURLAsync($"/reservations/{ValidReservation.Id}");
        }

        private async Task MockReservationsApiError()
        {
            await Page.RouteAsync("*/**/api/Reservation/me**", async route =>
            {
                await route.FulfillAsync(new() { Status = 400, Body = "Bad Request" });
            });
        }

        [Test]
        public async Task HasTabs()
        {
            await NavigateToUrl(UserReservationsUrl);
            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();
        }

        [Test]
        public async Task DoesNotHaveLegendComponent()
        {
            await NavigateToUrl(UserReservationsUrl);
            ILocator legend = Page.GetByTestId("custom-calendar-legend");
            await Expect(legend).ToHaveCountAsync(0);
        }

        [Test]
        public async Task HasCorrectAmountOfReservations()
        {
            await MockReservationsApi();
            await NavigateToUrl(UserReservationsUrl);

            await Expect(Page.GetByTestId("reservation-item")).ToHaveCountAsync(1);
        }

        [Test]
        public async Task ShowsReservations()
        {
            await MockReservationsApi();
            await NavigateToUrl(UserReservationsUrl);

            ILocator firstReservation = Page.GetByTestId("reservation-item").First;

            var date = firstReservation.GetByTestId("reservation-date");
            await Expect(date).ToContainTextAsync(ValidReservation.Date.ToString("dd/MM/yyyy"));

            var boatName = firstReservation.GetByTestId("reservation-boat-name");
            await Expect(boatName).ToContainTextAsync(ValidReservation.BoatPersonalName);

            var time = firstReservation.GetByTestId("reservation-time");
            await Expect(time).ToContainTextAsync($"{ValidReservation.Start:HH:mm} - {ValidReservation.End:HH:mm}");
        }


        [Test]
        public async Task ShowsLoadingStateWhileFetchingReservations()
        {
            await MockReservationsApi(delayMs: 5000);

            await NavigateToUrl(UserReservationsUrl);

            await Expect(Page.GetByTestId("user-reservations-loading-progress")).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShowsEmptyStateWhenNoReservations()
        {
            await MockEmptyReservationsApi();
            await NavigateToUrl(UserReservationsUrl);

            await Page.WaitForSelectorAsync("[data-testid='user-reservations-loading-progress']", new() { State = WaitForSelectorState.Hidden });
            await Page.WaitForSelectorAsync("[data-testid='no-reservations']", new() { State = WaitForSelectorState.Visible });

            ILocator emptyStateMessage = Page.GetByTestId("no-reservations");
            await Expect(emptyStateMessage).ToBeVisibleAsync();
            await Expect(emptyStateMessage).ToHaveTextAsync("U heeft geen aankomende reserveringen.");

            await Expect(Page.GetByTestId("reservation-item")).ToHaveCountAsync(0);
        }

        [Test]
        public async Task ShowsErrorStateWhenApiReturns400()
        {
            await MockReservationsApiError();
            await NavigateToUrl(UserReservationsUrl);

            await Page.WaitForSelectorAsync("[data-testid='user-reservations-loading-progress']", new() { State = WaitForSelectorState.Hidden });
            await Expect(Page.GetByTestId("user-reservations-fetch-error")).ToBeVisibleAsync();
            await Page.WaitForSelectorAsync("[data-testid='user-reservations-fetch-error']", new() { State = WaitForSelectorState.Visible });

            ILocator errorMessage = Page.GetByTestId("user-reservations-fetch-error");
            await Expect(errorMessage).ToBeVisibleAsync();

            await Expect(Page.GetByTestId("reservation-item")).ToHaveCountAsync(0);
        }

        [Test]
        public async Task ShowsPastReservations()
        {
            await MockPastReservationsApi();
            await NavigateToUrl(UserReservationsUrl + "&Past=true");

            ILocator locator = Page.GetByTestId("reservation-item");
            await Expect(locator).ToHaveCountAsync(1);
        }

        [Test]
        public async Task HasPastTab()
        {
            await NavigateToUrl(UserReservationsUrl);
            await Page.GetByTestId("tab-past-reservations").IsVisibleAsync();
        }


        [Test]
        public async Task CheckPastReservations()
        {
            await MockPastReservationsApi();
            await NavigateToUrl(UserReservationsUrl + "&Past=true");

            ILocator firstReservation = Page.GetByTestId("reservation-item").First;
            await Expect(firstReservation.GetByTestId("reservation-date")).Not.ToBeEmptyAsync();
            await Expect(firstReservation.GetByTestId("reservation-date")).ToContainTextAsync(PastReservation.Date.ToString("dd/MM/yyyy"));
            await Expect(firstReservation.GetByTestId("reservation-boat-name")).ToContainTextAsync(PastReservation.BoatPersonalName);
            await Expect(firstReservation.GetByTestId("reservation-time")).ToContainTextAsync($"{PastReservation.Start:HH:mm} - {PastReservation.End:HH:mm}");

        }


        [Test]
        public async Task TestTogglePastReservations()
        {

            await MockReservationsApi();
            await NavigateToUrl(UserReservationsUrl);


            ILocator upcomingReservation = Page.GetByTestId("reservation-item");
            // await Page.WaitForSelectorAsync("data-testid='reservation-item'");
            await Expect(upcomingReservation).ToHaveCountAsync(1);

            await MockPastReservationsApi();
            var toggleButton = Page.GetByTestId("reservation-toggle-button");
            await toggleButton.ClickAsync();

            // await Page.WaitForSelectorAsync("data-testid='reservation-item'");

            ILocator pastReservation = Page.GetByTestId("reservation-item");
            await Expect(pastReservation).ToHaveCountAsync(1);

        }

    }

}

