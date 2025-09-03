using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Playwright;
using Rise.Shared.Address;
using Rise.Shared.TimeSlots;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Reservations
{
    public class ReservationPageTest : CustomAuthenticatedPageTest
    {
        private const string universalDateFormat = "yyyy-MM-dd";

        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Member);
        }

        private static string DateToCalendarIdentifier(DateOnly date)
        {
            return $"[identifier='{date:d/MM/yyyy}']";
        }

        private readonly UserProfileDto profileDto = new()
        {
            Email = "guest@guest.com",
            FirstName = "Guest",
            FamilyName = "GuestFamilyName",
            PhoneNumber = "0123456789",
            Address = new AddressDto()
            {
                Street = "StreetName",
                City = "Belgium",
                Country = "CountryName",
                Number = "StreetNumber",
                PostalCode = "CityPostalCode",
            },
            DateOfBirth = new DateTime(2002, 2, 2)
        };

        private async Task MockProfileApi(int delayMs = 0)
        {
            await Page.RouteAsync($"*/**/api/User/profile**", async route =>
            {

                if (delayMs > 0)
                    await Task.Delay(delayMs);

                var response = profileDto;

                await route.FulfillAsync(new()
                {
                    ContentType = "application/json",
                    Body = System.Text.Json.JsonSerializer.Serialize(response)
                });
            });
        }

        private async Task MockAvailableDays()
        {
            int totalDays = 4;
            DateOnly startDate = new(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            DateOnly endDate = startDate.AddDays(totalDays);
            TimeSlotRangeInfoDto dto = new(
                TotalDays: totalDays,
                Days: [
                    new(startDate, false, false),
                    new(startDate.AddDays(1), true, false),
                    new(startDate.AddDays(2), false, true),
                    new(startDate.AddDays(3), true, true),
                ]
            );
            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(dto)
                });
            });
            await NavigateToUrl("/reservations");


        }

        private async Task MockTimeSlot()
        {
            TimeSlotDto[] timeSlotDtos = [
                new()
                {
                    Id = 1,
                    Start = new TimeOnly(9, 0, 0),
                    End = new TimeOnly(12, 0, 0),
                    IsBookedByUser = false
                },
                new()
                {
                    Id = 2,
                    Start = new TimeOnly(12, 0, 0),
                    End = new TimeOnly(15, 0, 0),
                    IsBookedByUser = false
                },
                new ()
                {
                    Id = 3,
                    Start = new TimeOnly(15, 0, 0),
                    End = new TimeOnly(18, 0, 0),
                    IsBookedByUser = true
                },
            ];

            await Page.RouteAsync("*/**/api/TimeSlot/*/*/**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(timeSlotDtos)
                });
            });
        }

        private async Task SelectAvailableDayOnCalendar()
        {
            // Arange
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            TimeSlotRangeInfoDto timeRange = new(
                TotalDays: 1,
                Days: [
                    new(today, false, true),
                ]
            );

            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(timeRange)
                });
            });

            await NavigateToUrl("/reservations");



            ILocator day = Page.Locator(DateToCalendarIdentifier(today));
            await day.ClickAsync();
        }

        private async Task MockTimeSlotAndSelectAvailableDay()
        {
            await MockTimeSlot();
            await MockProfileApi();
            await SelectAvailableDayOnCalendar();
        }

        [Test]
        public async Task HasTabs()
        {
            await NavigateToUrl("/reservations");
            await Page.GetByTestId("tab-reserve").IsVisibleAsync();
            await Page.GetByTestId("tab-your-reservations").IsVisibleAsync();
        }

        [Test]
        public async Task HasCustomCalendarReserveComponent()
        {
            await NavigateToUrl("/reservations");
            await Page.GetByTestId("custom-calendar-reserve").IsVisibleAsync();
        }

        [Test]
        public async Task HasLegendComponent()
        {
            await NavigateToUrl("/reservations");
            await Page.GetByTestId("custom-calendar-legend").IsVisibleAsync();
        }

        [Test]
        public async Task HasYourReservationsCalendarComponent()
        {
            await NavigateToUrl("/reservations");
            await Page.GetByTestId("calendar-your-reservations").IsVisibleAsync();
        }

        [Test]
        public async Task HasUnexpectedError()
        {
            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 400,
                    ContentType = "text/plain",
                    Body = "Bad argument!"
                });
            });
            await NavigateToUrl("/reservations");
            await Page.GetByTestId("custom-calendar-reserve-fetch-error").IsVisibleAsync();
        }

        [Test]
        public async Task ContainCalendarDataDates()
        {
            await MockAvailableDays();
            await NavigateToUrl("/reservations");

            ILocator available = Page.Locator("[data-celtype=available]");
            await Expect(available).ToHaveCountAsync(2);
        }

        [Test]
        public async Task ContainNoAvailableDateDates()
        {
            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(new List<object>())
                });
            });
            await NavigateToUrl("/reservations");

            ILocator booked = Page.Locator("[data-celtype=fully-booked]");
            try
            {
                await Expect(booked).ToHaveCountAsync(35);

            }
            catch (PlaywrightException)
            {
                await Expect(booked).ToHaveCountAsync(42);
            }
        }

        [Test]
        public async Task HasTimeSlotsInTimeSlotList()
        {
            await MockTimeSlotAndSelectAvailableDay();

            ILocator timeSlotList = Page.GetByTestId("time-slot-list");

            ILocator timeSlot1 = timeSlotList.GetByTestId("time-slot-1");
            ILocator timeSlot2 = timeSlotList.GetByTestId("time-slot-2");
            ILocator timeSlot3 = timeSlotList.GetByTestId("time-slot-3");


            // Assert

            await Expect(timeSlot1).ToHaveCountAsync(1);
            await Expect(timeSlot2).ToHaveCountAsync(1);
            await Expect(timeSlot3).ToHaveCountAsync(1);

            // Assert the styles
            // TODO assert type of time slots
            // await Expect(timeSlot1).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-dark-rgb), 0.1);");
            // await Expect(timeSlot2).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-dark-rgb), 0.1);");
            // await Expect(timeSlot3).ToHaveAttributeAsync("style", "background-color:rgba(var(--mud-palette-primary-rgb), 0.1);");
        }

        [Test]
        public async Task ShouldHaveUnexpectedErrorForTimeSlotList()
        {

            await Page.RouteAsync("*/**/api/TimeSlot/*/*/**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 400,
                    ContentType = "text/plain",
                    Body = "Bad Request"
                });
            });

            await SelectAvailableDayOnCalendar();

            await Expect(Page.GetByTestId("time-slot-list-fetch-error")).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldHaveLoaderForTimeSlotList()
        {

            await Page.RouteAsync("*/**/api/TimeSlot/*/*/**", async route =>
            {
                await Task.Delay(3000);
                await route.FulfillAsync(new()
                {
                    Status = 400,
                    ContentType = "text/plain",
                    Body = "Bad Request"
                });
            });

            await SelectAvailableDayOnCalendar();
            await Page.WaitForLoadStateAsync(LoadState.DOMContentLoaded);
            await Page.GetByTestId("time-slot-list-loading-progress").IsVisibleAsync();
        }

        [Test]
        public async Task ShouldHaveEmptyMessageForTimeSlotList()
        {

            await Page.RouteAsync("*/**/api/TimeSlot/*/*/**", async route =>
            {
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(new List<TimeSlotDto>())
                });
            });

            await SelectAvailableDayOnCalendar();

            await Page.GetByTestId("time-slots-none").IsVisibleAsync();
        }

        [Test]
        public async Task ShouldNotBeAbleToGoBackToPreviousMonthFromCurrentUsingButtons()
        {

            await NavigateToUrl("/reservations");
            ILocator prev = Page.GetByTestId("calendar-previous");
            await Expect(prev).ToBeDisabledAsync();
        }

        [Test]
        public async Task ShouldNotBeAbleToGoBackToPreviousMonthFromCurrentUsingPicker()
        {

            await NavigateToUrl("/reservations");

            ILocator monthPicker = Page.Locator(".mud-picker.mud-picker-inline.mud-picker-input-button");
            await monthPicker.ClickAsync();

            int currentMonth = DateTime.Now.Month;
            ILocator monthPickerCollapsed = Page.GetByTestId("calendar-datepicker");

            if (currentMonth == 1)
            {
                await monthPickerCollapsed.GetByLabel($"Previous year ({DateTime.Today.AddYears(-1).Year})").ClickAsync();
                currentMonth = 13;
            }

            ILocator previousMonth = monthPickerCollapsed.Locator(".mud-picker-month").Nth(currentMonth - 2);
            await Expect(previousMonth).ToBeDisabledAsync();
        }

        [Test]
        public async Task ShouldBeAbleToGoBackToPreviousMonthFromNextMonthUsingButtons()
        {
            await NavigateToUrl("/reservations");

            ILocator monthPicker = Page.Locator(".mud-picker.mud-picker-inline.mud-picker-input-button");
            string startMonthText = await monthPicker.InnerTextAsync();

            ILocator next = Page.GetByTestId("calendar-next");
            await next.ClickAsync();
            await Expect(monthPicker).Not.ToHaveTextAsync(new Regex(startMonthText.ToLower(), RegexOptions.IgnoreCase));

            ILocator prev = Page.GetByTestId("calendar-previous");
            await Expect(prev).Not.ToBeDisabledAsync();
            await prev.ClickAsync();

            await Expect(monthPicker).ToHaveTextAsync(new Regex(startMonthText.ToLower(), RegexOptions.IgnoreCase));
        }

        [Test]
        public async Task ShouldBeAbleToGoBackToPreviousMonthFromNextMonthUsingDatePicker()
        {
            await NavigateToUrl("/reservations");

            ILocator monthPicker = Page.Locator(".mud-picker.mud-picker-inline.mud-picker-input-button");
            string startMonthText = await monthPicker.InnerTextAsync();

            ILocator next = Page.GetByTestId("calendar-next");
            await next.ClickAsync();
            await Expect(monthPicker).Not.ToHaveTextAsync(new Regex(startMonthText.ToLower(), RegexOptions.IgnoreCase));

            await monthPicker.ClickAsync();

            int nextMonth = (DateTime.Now.Month + 1) % 12;
            ILocator monthPickerCollapsed = Page.GetByTestId("calendar-datepicker");

            if (nextMonth == 1)
            {
                await monthPickerCollapsed.GetByLabel($"Previous year ({DateTime.Today.Year})").ClickAsync();
                nextMonth = 13;
            }

            ILocator previousMonth = monthPickerCollapsed.Locator(".mud-picker-month").Nth(nextMonth - 2);
            await Expect(previousMonth).Not.ToBeDisabledAsync();
            await previousMonth.ClickAsync();

            await Expect(monthPicker).ToHaveTextAsync(new Regex(startMonthText.ToLower(), RegexOptions.IgnoreCase));
        }

        [Test]
        public async Task ShouldRedirectToThisMonthsCurrentDateWhenNoCurrentDate()
        {
            string currentDate = DateTime.Today.ToString(universalDateFormat);
            await NavigateToUrl("/reservations");
            // await Page.WaitForFunctionAsync($"() => window.location.href.includes('CurrentDate={currentDate}')");
            await Expect(Page).ToHaveURLAsync(new Regex($"\\?.*CurrentDate={currentDate}"));
        }

        [Test]
        public async Task ShouldRedirectToThisMonthsCurrentDateWhenToEarlyDate()
        {
            string toEarlyDate = DateTime.Today.AddDays(-1).ToString(universalDateFormat);
            string currentDate = DateTime.Today.ToString(universalDateFormat);
            await NavigateToUrl($"/reservations?CurrentDate={toEarlyDate}");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('CurrentDate={currentDate}')");
            await Expect(Page).ToHaveURLAsync(new Regex($"\\?.*CurrentDate={currentDate}"));
        }

        [Test]
        public async Task ShouldRedirectToGivenCurrentDate()
        {
            DateTime plusOneMonthDate = DateTime.Today.AddMonths(1);
            string plusOneMonthDateFormatted = plusOneMonthDate.ToString(universalDateFormat);
            await NavigateToUrl($"/reservations?CurrentDate={plusOneMonthDateFormatted}");
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('CurrentDate={plusOneMonthDateFormatted}')");
            await Expect(Page).ToHaveURLAsync(new Regex($"\\?.*CurrentDate={plusOneMonthDateFormatted}"));

            ILocator date = Page.Locator(DateToCalendarIdentifier(DateOnly.FromDateTime(plusOneMonthDate)));
            await Expect(date).ToHaveCountAsync(1);
        }

        [Test]
        public async Task ShouldChangeCurrentDateWhenGoingToNextMonth()
        {
            await NavigateToUrl("/reservations");
            ILocator next = Page.GetByTestId("calendar-next");
            await next.ClickAsync();
            string nextMonthDate = DateTime.Today.AddMonths(1).ToString(universalDateFormat);
            await Page.WaitForFunctionAsync($"() => window.location.href.includes('CurrentDate={nextMonthDate}')");
            await Expect(Page).ToHaveURLAsync(new Regex($"\\?.*CurrentDate={nextMonthDate}"));
        }


        [Test]
        public async Task FullyBookedDaysAreUnclickable()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            await NavigateToUrl("/reservations");

            ILocator timeSlotList = Page.GetByTestId("time-slot-list");
            await Expect(timeSlotList).ToHaveCountAsync(0);

            ILocator calendarCellToday = Page.Locator(DateToCalendarIdentifier(today));
            await calendarCellToday.ClickAsync();

            await Expect(timeSlotList).ToHaveCountAsync(0);
        }

        [Test]
        public async Task SelectingFullyBookedDayShouldRemoveExistingSelectedDay()
        {
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);

            await MockTimeSlotAndSelectAvailableDay();

            ILocator timeSlotList = Page.GetByTestId("time-slot-list");
            await Expect(timeSlotList).ToHaveCountAsync(1);

            DateOnly day = new(today.Year, today.Month, 1);
            if (today.Day == 1) day = day.AddDays(1);

            ILocator calendarCellToday = Page.Locator(DateToCalendarIdentifier(day));
            await calendarCellToday.ClickAsync();

            await Expect(timeSlotList).ToHaveCountAsync(0);
        }

        [Test]
        public async Task ReservationDialogShouldCloseWhenCloseButtonIsClicked()
        {
            //Arrange
            await MockProfileApi();
            await OpenCreateReservationDialog();

            ILocator dialog = Page.GetByTestId("reservation-dialog");
            ILocator closeButton = Page.GetByTestId("dialog-cancel-button");
            await closeButton.ClickAsync();

            await Expect(dialog).Not.ToBeVisibleAsync();
        }

        [Test]
        public async Task ReservationDialogShouldProceedWhenCreateButtonIsClicked()
        {
            //Arrange
            await MockProfileApi();
            await OpenCreateReservationDialog();

            // Act
            // ILocator dialog = Page.GetByTestId("reservation-dialog");
            ILocator closeButton = Page.GetByTestId("dialog-create-button");
            await closeButton.ClickAsync();

            // Assert
            var dialogPaymentContent = Page.GetByTestId("dialog-payment-content");
            await Expect(dialogPaymentContent).ToBeVisibleAsync();

            ILocator dialogSuccessContent = Page.GetByTestId("dialog-success-content");
            await Expect(dialogSuccessContent).ToBeVisibleAsync();
        }

        [Test]
        public async Task CreatingReservationShouldDisplayReservationOnCalendar()
        {
            await MockProfileApi();
            await OpenCreateReservationDialog();
            var today = DateOnly.FromDateTime(DateTime.Now);
            // Create the reservation
            ILocator createButton = Page.GetByTestId("dialog-create-button");
            await createButton.ClickAsync();

            // Wait for payment and success states
            await Page.WaitForSelectorAsync("[data-testid='dialog-payment-content']", new() { State = WaitForSelectorState.Visible });


            ILocator dialogSuccessContent = Page.GetByTestId("dialog-success-content");
            await Expect(dialogSuccessContent).ToBeVisibleAsync();

            // Close the dialog
            ILocator closeButton = Page.GetByTestId("dialog-close-button");
            await closeButton.ClickAsync();

            // Assert
            // Check if the calendar shows the updated state
            ILocator calendarCell = Page.Locator(DateToCalendarIdentifier(today));
            await Expect(calendarCell).ToBeVisibleAsync();

            ILocator svgElement = calendarCell.Locator("svg");
            await Expect(svgElement).ToBeVisibleAsync();
        }

        private async Task OpenCreateReservationDialog()
        {
            // Arrange
            DateOnly today = DateOnly.FromDateTime(DateTime.Now);
            TimeSlotRangeInfoDto initialTimeRange = new(
                TotalDays: 1,
                Days: [
                    new(today, false, true),
                ]
            );

            TimeSlotDto[] initialTimeSlots = [
                new()
                {
                    Id = 1,
                    Start = new TimeOnly(9, 0, 0),
                    End = new TimeOnly(12, 0, 0),
                    IsBookedByUser = false
                },
                new()
                {
                    Id = 2,
                    Start = new TimeOnly(12, 0, 0),
                    End = new TimeOnly(15, 0, 0),
                    IsBookedByUser = false
                },
                new()
                {
                    Id = 3,
                    Start = new TimeOnly(15, 0, 0),
                    End = new TimeOnly(18, 0, 0),
                    IsBookedByUser = true
                },
            ];

            // Updated data that will be returned after creating reservation
            TimeSlotRangeInfoDto updatedTimeRange = new(
                TotalDays: 1,
                Days: [
                    new(today, true, true, true), // Updated to show user has a booking
                ]
            );

            TimeSlotDto[] updatedTimeSlots = [
                new()
                {
                    Id = 1,
                    Start = new TimeOnly(9, 0, 0),
                    End = new TimeOnly(12, 0, 0),
                    IsBookedByUser = true  // This slot is now booked by the user
                },
                new()
                {
                    Id = 2,
                    Start = new TimeOnly(12, 0, 0),
                    End = new TimeOnly(15, 0, 0),
                    IsBookedByUser = false
                },
                new()
                {
                    Id = 3,
                    Start = new TimeOnly(15, 0, 0),
                    End = new TimeOnly(18, 0, 0),
                    IsBookedByUser = true
                },
            ];

            bool hasCreatedReservation = false;

            // Mock initial GET requests
            await Page.RouteAsync("*/**/api/TimeSlot/range**", async route =>
            {
                var response = hasCreatedReservation ? updatedTimeRange : initialTimeRange;
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(response)
                });
            });

            await Page.RouteAsync("*/**/api/TimeSlot/*/*/**", async route =>
            {
                var response = hasCreatedReservation ? updatedTimeSlots : initialTimeSlots;
                await route.FulfillAsync(new()
                {
                    Status = 200,
                    ContentType = "text/json",
                    Body = JsonSerializer.Serialize(response)
                });
            });

            // Mock POST request for creating reservation
            await Page.RouteAsync("*/**/api/Reservation/", async route =>
            {
                if (route.Request.Method == "POST")
                {
                    hasCreatedReservation = true;
                    await route.FulfillAsync(new()
                    {
                        Status = 200,
                        ContentType = "application/json",
                        Body = JsonSerializer.Serialize(1)
                    });
                }
            });

            // Act
            await NavigateToUrl("/reservations");

            // Select the day and time slot
            ILocator day = Page.Locator(DateToCalendarIdentifier(today));
            await day.ClickAsync();

            ILocator timeSlot1 = Page.GetByTestId("time-slot-1");
            await timeSlot1.ClickAsync();

            // Wait for dialog to appear
            await Page.WaitForSelectorAsync("[data-testid='reservation-dialog']", new() { State = WaitForSelectorState.Visible });
        }
    }
}