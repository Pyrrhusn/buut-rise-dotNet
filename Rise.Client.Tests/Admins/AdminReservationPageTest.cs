using System.Text.RegularExpressions;
using Rise.Shared.Pagination;
using Rise.Shared.Reservations;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Admin;


[TestFixture]
public class AdminReservationPageTest : CustomAuthenticatedPageTest
{
    [SetUp]
    public async Task SetUpAsync()
    {
        await LoginAsync(UserRole.Administrator);
    }

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

    private async Task MockReservationsApi(ItemsPageDto<ReservationDto>? response = null, int status = 200)
    {
        await Page.RouteAsync("**/api/Reservation/all?**", async route =>
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
    public async Task Admin_CancelsReservationSuccessfully_SameDay()
    {
        var reservations = new ItemsPageDto<ReservationDto>
        {
            IsFirstPage = true,
            NextId = 11,
            PreviousId = 9,

            Data = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 10,
                Date = DateOnly.FromDateTime(DateTime.Today),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatPersonalName = "Ocean Wave",
                UserName = "Alice Smith",
                IsDeleted = false
            }
        }
        };

        await MockReservationsApi(reservations);
        await MockCancelReservationApi(10);

        await NavigateToUrl("/admin/reservations");
        var cancelButton = Page.GetByTestId("admin-reservation-cancel-button");
        await cancelButton.ClickAsync();

        await Expect(Page).ToHaveURLAsync("/admin/reservations");
    }

    [Test]
    public async Task Admin_PreventedFromCancelingPastReservation()
    {
        var reservations = new ItemsPageDto<ReservationDto>
        {
            IsFirstPage = true,
            NextId = 9,
            PreviousId = 11,


            Data = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 11,
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(-1)),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatPersonalName = "Sea Whisper",
                UserName = "Bob Johnson",
                IsDeleted = false
            }
        }
        };
        await MockReservationsApi(reservations);

        await NavigateToUrl("/admin/reservations");

        var toggleButton = Page.GetByTestId("admin-reservations-toggle-button");
        await toggleButton.ClickAsync();

        var cancelButton = Page.GetByTestId("admin-reservation-cancel-button");
        await cancelButton.ClickAsync();

        var errorMessage = Page.GetByTestId("admin-cancel-reservation-error");
        await Expect(errorMessage).ToBeVisibleAsync();
        await Expect(errorMessage).ToContainTextAsync("Failed to cancel reservation with ID");
    }

    [Test]
    public async Task Admin_CannotCancelAlreadyCancelledReservation()
    {
        var reservations = new ItemsPageDto<ReservationDto>
        {
            Data = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 12,
                Date = DateOnly.Parse(DateTime.Now.AddDays(5).ToString("yyyy/MM/dd")),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatPersonalName = "Blue Lagoon",
                UserName = "Chris Thompson",
                IsDeleted = true
            }
        }
        };

        await MockReservationsApi(reservations);

        await NavigateToUrl("/admin/reservations");
        var cancelButton = Page.GetByTestId("admin-reservation-status");
        await Expect(cancelButton).ToHaveTextAsync("Geannuleerd");
    }

    [Test]
    public async Task Admin_CancelsReservationWithinTwoDays_Successfully()
    {
        var reservations = new ItemsPageDto<ReservationDto>
        {
            Data = new List<ReservationDto>
        {
            new ReservationDto
            {
                Id = 13,
                Date = DateOnly.Parse(DateTime.Now.AddDays(1).ToString("yyyy/MM/dd")),
                Start = TimeOnly.Parse("10:00"),
                End = TimeOnly.Parse("13:00"),
                BoatPersonalName = "Mystic River",
                UserName = "David Parker",
                IsDeleted = false
            }
        }
        };

        await MockReservationsApi(reservations);
        await MockCancelReservationApi(13);

        await NavigateToUrl("/admin/reservations");
        var cancelButton = Page.GetByTestId("admin-reservation-cancel-button");
        await cancelButton.ClickAsync();

        await Expect(Page).ToHaveURLAsync("/admin/reservations");
    }






}
