using System.Net;
using System.Text.Json;
using Microsoft.Playwright;
using Rise.Shared.TimeSlots;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Admins.CruisePeriods
{
    public class AdminTimeSlotsPageTestAdmin : CustomAuthenticatedPageTest
    {
        private const string baseSuffix = "/admin/cruise_period/1/timeslots";

        [SetUp]
        public async Task SetUp()
        {
            await LoginAsync(UserRole.Administrator);
        }

        private static RouteFulfillOptions FulfillWithOkResponse(RouteFulfillOptions fulfillOptions, object Data)
        {
            fulfillOptions.Status = (int)HttpStatusCode.OK;
            fulfillOptions.ContentType = "text/json";
            fulfillOptions.Body = JsonSerializer.Serialize(Data);
            return fulfillOptions;
        }

        private static RouteFulfillOptions FulfillWithBadRequestResponse(RouteFulfillOptions fulfillOptions, string message = "Bad Request")
        {
            fulfillOptions.Status = (int)HttpStatusCode.BadRequest;
            fulfillOptions.ContentType = "text/json";
            fulfillOptions.Body = JsonSerializer.Serialize(message);
            return fulfillOptions;
        }

        private async Task MockCruisePeriodDetails(int cruisePeriodId, CruisePeriodDetailedDto cruisePeriod)
        {
            await Page.RouteAsync($"**/api/CruisePeriod/{cruisePeriodId}", async route =>
            {
                var fulfillOptions = new RouteFulfillOptions();
                if (route.Request.Method == "GET")
                {
                    fulfillOptions = FulfillWithOkResponse(fulfillOptions, cruisePeriod);
                }
                await route.FulfillAsync(fulfillOptions);
            });
        }

        private async Task MockTimeSlotCreation(bool success = true, string errorMessage = "")
        {
            await Page.RouteAsync("**/api/TimeSlot", async route =>
            {
                var fulfillOptions = new RouteFulfillOptions();
                if (route.Request.Method == "POST")
                {
                    if (success)
                    {
                        fulfillOptions = FulfillWithOkResponse(fulfillOptions, new { Success = true });
                    }
                    else
                    {
                        fulfillOptions = FulfillWithBadRequestResponse(fulfillOptions, errorMessage);
                    }
                }
                await route.FulfillAsync(fulfillOptions);
            });
        }



        [Test]
        public async Task ShouldLoadCruisePeriodDetails()
        {
            var cruisePeriod = new CruisePeriodDetailedDto
            {
                Id = 1,
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(7)
            };

            await MockCruisePeriodDetails(cruisePeriod.Id, cruisePeriod);
            await NavigateToUrl(baseSuffix);

            var periodName = Page.GetByTestId("cruise-period-name");
            await Expect(periodName).ToBeVisibleAsync();

            var startDate = Page.GetByTestId("cruise-period-start");
            await Expect(startDate).ToBeVisibleAsync();
            await Expect(startDate).ToContainTextAsync(cruisePeriod.Start.ToString("dd/MM/yyyy"));

            var endDate = Page.GetByTestId("cruise-period-end");
            await Expect(endDate).ToBeVisibleAsync();
            await Expect(endDate).ToContainTextAsync(cruisePeriod.End.ToString("dd/MM/yyyy"));
        }


        [Test]
        public async Task ShouldAddAndSaveTimeSlots()
        {

            var cruisePeriod = new CruisePeriodDetailedDto
            {
                Id = 1,
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(7)
            };

            await MockCruisePeriodDetails(cruisePeriod.Id, cruisePeriod);
            await MockTimeSlotCreation();
            await NavigateToUrl(baseSuffix);

            var noSlotsMessage = Page.GetByTestId("no-time-slots-message");
            await Expect(noSlotsMessage).ToBeVisibleAsync();

            await SelectTimeByJavaScript("time-slot-start", "09:00");
            await SelectTimeByJavaScript("time-slot-end", "12:00");
            var addButton = Page.GetByTestId("add-time-slot-button");
            await addButton.ClickAsync();
        }
        private async Task SelectTimeByJavaScript(string testId, string time)
        {
            //mudblazor time picker is not working with playwright
            //so used javascript to set the value
            await Page.EvalOnSelectorAsync($"[data-testid='{testId}']",
                $"el => {{ el.value = '{time}'; el.dispatchEvent(new Event('change')); }}");
        }
        [Test]
        public async Task ShouldDisplayNoTimeSlotsMessageInitially()
        {
            var cruisePeriod = new CruisePeriodDetailedDto
            {
                Id = 1,
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(7)
            };

            await MockCruisePeriodDetails(cruisePeriod.Id, cruisePeriod);
            await NavigateToUrl(baseSuffix);

            var noSlotsMessage = Page.GetByTestId("no-time-slots-message");
            await Expect(noSlotsMessage).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldAddTimeSlotSuccessfully()
        {
            var cruisePeriod = new CruisePeriodDetailedDto
            {
                Id = 1,
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(7)
            };

            await MockCruisePeriodDetails(cruisePeriod.Id, cruisePeriod);
            await MockTimeSlotCreation();
            await NavigateToUrl(baseSuffix);

            await SelectTimeByJavaScript("time-slot-start", "09:00");
            await SelectTimeByJavaScript("time-slot-end", "12:00");
            await Page.GetByTestId("add-time-slot-button").ClickAsync();

            var timeSlotsList = Page.GetByTestId("time-slots-list");
            await Expect(timeSlotsList).ToBeVisibleAsync();

            var timeSlotText = Page.GetByTestId("time-slot-text-09:00 - 12:00");
            await Expect(timeSlotText).ToBeVisibleAsync();
            await Expect(timeSlotText).ToContainTextAsync("09:00 - 12:00");
        }

        [Test]
        public async Task ShouldHandleTimeSlotCreationError()
        {
            var cruisePeriod = new CruisePeriodDetailedDto
            {
                Id = 1,
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(7)
            };

            await MockCruisePeriodDetails(cruisePeriod.Id, cruisePeriod);
            await MockTimeSlotCreation(success: false, errorMessage: "Invalid time slot");
            await NavigateToUrl(baseSuffix);

            await SelectTimeByJavaScript("time-slot-start", "09:00");
            await SelectTimeByJavaScript("time-slot-end", "08:00");
            await Page.GetByTestId("add-time-slot-button").ClickAsync();

            var noSlotsMessage = Page.GetByTestId("no-time-slots-message");
            await Expect(noSlotsMessage).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldRemoveTimeSlotSuccessfully()
        {
            var cruisePeriod = new CruisePeriodDetailedDto
            {
                Id = 1,
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(7)
            };

            await MockCruisePeriodDetails(cruisePeriod.Id, cruisePeriod);
            await MockTimeSlotCreation();
            await NavigateToUrl(baseSuffix);

            await SelectTimeByJavaScript("time-slot-start", "09:00");
            await SelectTimeByJavaScript("time-slot-end", "12:00");
            await Page.GetByTestId("add-time-slot-button").ClickAsync();

            var timeSlotText = Page.GetByTestId("time-slot-text-09:00 - 12:00");
            await Expect(timeSlotText).ToBeVisibleAsync();

            await Page.GetByTestId("delete-time-slot-button").ClickAsync();

            await Expect(timeSlotText).Not.ToBeVisibleAsync();
            var noSlotsMessage = Page.GetByTestId("no-time-slots-message");
            await Expect(noSlotsMessage).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldEnableSaveButtonWhenTimeSlotsExist()
        {
            var cruisePeriod = new CruisePeriodDetailedDto
            {
                Id = 1,
                Start = DateTime.Today,
                End = DateTime.Today.AddDays(7)
            };

            await MockCruisePeriodDetails(cruisePeriod.Id, cruisePeriod);
            await MockTimeSlotCreation();
            await NavigateToUrl(baseSuffix);

            var saveButton = Page.GetByTestId("save-time-slots-button");
            await Expect(saveButton).ToBeDisabledAsync();

            await SelectTimeByJavaScript("time-slot-start", "09:00");
            await SelectTimeByJavaScript("time-slot-end", "12:00");
            await Page.GetByTestId("add-time-slot-button").ClickAsync();

            await Expect(saveButton).ToBeEnabledAsync();
        }


    }
}