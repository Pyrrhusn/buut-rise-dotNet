using System.Text.Json;
using System.Text.RegularExpressions;
using Rise.Shared.Reservations;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Reservations
{
    public class ReservationDetailsPageTest : CustomAuthenticatedPageTest
    {
        private const string UserReservationDetailsUrl = "/reservations/1";
        private const string InvalidReservationDetailsUrl = "/reservations/100";

        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Member);
        }

        private async Task MockReservationDetailsApi(ReservationDetailsDto? response = null, int status = 200)
        {
            await Page.RouteAsync(new Regex("^.*/api/Reservation/\\d+"), async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = status,
                    ContentType = "application/json",
                    Body = response != null ? JsonSerializer.Serialize(response) : string.Empty
                });
            });
        }

        [Test]
        public async Task ShowsReservationDetails()
        {
            ReservationDetailsDto reservationDetails = new()
            {
                Id = 1,
                Date = new DateOnly(2024, 10, 30),
                Start = new TimeOnly(10, 0, 0),
                End = new TimeOnly(13, 0, 0),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                BatteryType = "Lithium-Ion"
            };

            await MockReservationDetailsApi(reservationDetails);
            await NavigateToUrl(UserReservationDetailsUrl);

            await Expect(Page.GetByTestId("reservation-details-date")).ToContainTextAsync(reservationDetails.Date.ToString("dd/MM/yyyy"));
            await Expect(Page.GetByTestId("reservation-details-boat")).ToContainTextAsync(reservationDetails.BoatPersonalName);
            await Expect(Page.GetByTestId("reservation-details-time")).ToContainTextAsync($"{reservationDetails.Start:HH:mm} - {reservationDetails.End:HH:mm}");
        }

        [Test]
        public async Task ShowsNotFoundErrorForNonExistentReservation()
        {
            await MockReservationDetailsApi(null, status: 404);
            await NavigateToUrl(UserReservationDetailsUrl);

            var errorMessage = Page.Locator("text='Response status code does not indicate success: 404 (Not Found).'");

            await Expect(errorMessage).ToBeVisibleAsync(new() { Timeout = 30000 });
        }


        [Test]
        public async Task ShowsCurrentBatteryUserWhenAvailable()
        {
            var reservationDetails = new ReservationDetailsDto
            {
                Id = 1,
                Date = DateOnly.Parse("2024/10/30"),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                BatteryType = "Lithium-Ion",
                CurrentBatteryUserId = 42,
                CurrentBatteryUserName = "Jane Smith",
                CurrentHolderPhoneNumber = "123456789",
                CurrentHolderEmail = "jane@example.com",
                CurrentHolderStreet = "Main Street",
                CurrentHolderNumber = "123",
                CurrentHolderPostalCode = "1000",
                CurrentHolderCity = "Brussels"
            };

            await MockReservationDetailsApi(reservationDetails);
            await NavigateToUrl(UserReservationDetailsUrl);

            await Expect(Page.GetByTestId("reservation-battery-current-user")).ToContainTextAsync(reservationDetails.CurrentBatteryUserName);
            await Expect(Page.GetByTestId("reservation-holder-phone")).ToContainTextAsync(reservationDetails.CurrentHolderPhoneNumber);
            await Expect(Page.GetByTestId("reservation-holder-email")).ToContainTextAsync(reservationDetails.CurrentHolderEmail);
            await Expect(Page.GetByTestId("reservation-holder-address")).ToContainTextAsync($"{reservationDetails.CurrentHolderStreet} {reservationDetails.CurrentHolderNumber}");
            await Expect(Page.GetByTestId("reservation-holder-city")).ToContainTextAsync($"{reservationDetails.CurrentHolderPostalCode} {reservationDetails.CurrentHolderCity}");
        }

        [Test]
        public async Task ShowsNoPickupInfoWhenNoCurrentHolder()
        {
            var reservationDetails = new ReservationDetailsDto
            {
                Id = 1,
                Date = DateOnly.Parse("2024/10/30"),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                // No holder details provided
            };

            await MockReservationDetailsApi(reservationDetails);
            await NavigateToUrl(UserReservationDetailsUrl);

            await Expect(Page.GetByTestId("no-pickup-info")).ToContainTextAsync("Geen ophaal informatie beschikbaar");
        }
    }
}