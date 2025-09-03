using System.Text.RegularExpressions;
using Rise.Shared.Reservations;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Reservations;

public class CancelReservationTest : CustomAuthenticatedPageTest
{
    [SetUp]
    public async Task SetUp()
    {
        await LoginAsync(UserRole.Member);
    }

    private const string UserReservationDetailsUrl = "/reservations/41";
    private const string ReservationsListUrl = "/reservations?CurrentTab=reservations";

    private async Task MockCancelReservationApi(int reservationId, int status = 200)
    {
        await Page.RouteAsync($"**/api/Reservation/cancel/{reservationId}", async route =>
        {
            await route.FulfillAsync(new()
            {
                Status = status,
                ContentType = "application/json",
                Body = string.Empty
            });
        });
    }

    private async Task MockReservationDetailsApi(ReservationDetailsDto? response = null, int status = 200)
    {
        await Page.RouteAsync(new Regex("^.*/api/Reservation/\\d+"), async route =>
        {
            await route.FulfillAsync(new()
            {
                Status = status,
                ContentType = "application/json",
                Body = response != null ? System.Text.Json.JsonSerializer.Serialize(response) : string.Empty
            });
        });
    }

    [Test]
    public async Task ShowMessageForCancelledReservation()
    {
        // Arrange:
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 3,
            Date = DateOnly.Parse("2024/12/05"),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 3,
            BoatPersonalName = "Sea Breeze",
            MentorName = "John Smith",
            BatteryType = "Lead-Acid",
            IsDeleted = true
        };

        await MockReservationDetailsApi(reservationDetails);

        await NavigateToUrl($"/reservations/{reservationDetails.Id}");

        var cancelledMessage = Page.GetByTestId("cancel-reservation-geannuleerd");
        await Expect(cancelledMessage).ToBeVisibleAsync();
        await Expect(cancelledMessage).ToHaveTextAsync("Deze reservatie is geannuleerd. Je kan de details niet bekijken.");
    }

    [Test]
    public async Task ShowsCancelConfirmationDialog()
    {
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 1,
            Date = DateOnly.Parse(DateTime.Now.AddDays(5).ToString("yyyy/MM/dd")),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 1,
            BoatPersonalName = "Limba",
            MentorName = "John Doe",
            BatteryType = "Lithium-Ion",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);
        await NavigateToUrl($"/reservations/{reservationDetails.Id}");

        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();

        // Verify dialog appears with correct content
        var dialogTitle = Page.GetByText("Reservatie annuleren");
        var dialogContent = Page.GetByText("Weet u zeker dat u deze reservatie wilt annuleren?");
        
        await Expect(dialogTitle).ToBeVisibleAsync();
        await Expect(dialogContent).ToBeVisibleAsync();
    }

    [Test]
    public async Task CancelsReservationWhenConfirmed()
    {
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 1,
            Date = DateOnly.Parse(DateTime.Now.AddDays(5).ToString("yyyy/MM/dd")),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 1,
            BoatPersonalName = "Limba",
            MentorName = "John Doe",
            BatteryType = "Lithium-Ion",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);
        await MockCancelReservationApi(reservationDetails.Id);

        await NavigateToUrl($"/reservations/{reservationDetails.Id}");

        // Click cancel button to show dialog
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();

        // Click confirm in dialog
        var confirmButton = Page.GetByTestId("cancel-confirmation-yes");
        await confirmButton.ClickAsync();

        // Should redirect to reservations list
        await Expect(Page).ToHaveURLAsync(ReservationsListUrl);
    }

    [Test]
    public async Task DoesNotCancelWhenDialogCancelled()
    {
        var reservationDetails = new ReservationDetailsDto
        {
            Id = 1,
            Date = DateOnly.Parse(DateTime.Now.AddDays(5).ToString("yyyy/MM/dd")),
            Start = TimeOnly.Parse("10:00"),
            End = TimeOnly.Parse("13:00"),
            BoatId = 1,
            BoatPersonalName = "Limba",
            MentorName = "John Doe",
            BatteryType = "Lithium-Ion",
            IsDeleted = false
        };

        await MockReservationDetailsApi(reservationDetails);
        await NavigateToUrl($"/reservations/{reservationDetails.Id}");

        // Click cancel button to show dialog
        var cancelButton = Page.GetByTestId("cancel-reservation-button");
        await cancelButton.ClickAsync();

        // Click no in dialog
        var noButton = Page.GetByTestId("cancel-confirmation-no");
        await noButton.ClickAsync();

        // Should stay on same page
        await Expect(Page).ToHaveURLAsync($"/reservations/{reservationDetails.Id}");
    }

           [Test]
        public async Task ShowsCancelButtonForValidReservation()
        {
            var reservationDetails = new ReservationDetailsDto
            {
                Id = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                BatteryType = "Lithium-Ion",
                IsDeleted = false
            };

            await MockReservationDetailsApi(reservationDetails);
            await NavigateToUrl(UserReservationDetailsUrl);

            await Expect(Page.GetByTestId("cancel-reservation-button")).ToBeVisibleAsync();
        }

        [Test]
        public async Task HidesCancelButtonForDeletedReservation()
        {
            var reservationDetails = new ReservationDetailsDto
            {
                Id = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(3)),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                BatteryType = "Lithium-Ion",
                IsDeleted = true
            };

            await MockReservationDetailsApi(reservationDetails);
            await NavigateToUrl(UserReservationDetailsUrl);

            await Expect(Page.GetByTestId("cancel-reservation-button")).Not.ToBeVisibleAsync();
        }

        [Test]
        public async Task HidesCancelButtonForPastReservation()
        {
            var reservationDetails = new ReservationDetailsDto
            {
                Id = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                BatteryType = "Lithium-Ion",
                IsDeleted = false
            };

            await MockReservationDetailsApi(reservationDetails);
            await NavigateToUrl(UserReservationDetailsUrl);

            await Expect(Page.GetByTestId("cancel-reservation-button")).Not.ToBeVisibleAsync();
        }

        [Test]
        public async Task HidesCancelButtonForReservationLessThanTwoDaysAway()
        {
            var reservationDetails = new ReservationDetailsDto
            {
                Id = 1,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(1)),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatId = 101,
                BoatPersonalName = "Limba",
                MentorName = "John Doe",
                BatteryType = "Lithium-Ion",
                IsDeleted = false
            };

            await MockReservationDetailsApi(reservationDetails);
            await NavigateToUrl(UserReservationDetailsUrl);

            await Expect(Page.GetByTestId("cancel-reservation-button")).Not.ToBeVisibleAsync();
        }
}

