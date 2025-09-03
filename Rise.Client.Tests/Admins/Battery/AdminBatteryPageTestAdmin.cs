using System.Net;
using System.Text.Json;
using System.Web;
using Microsoft.Playwright;
using Rise.Shared.Boats;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Admins.Battery
{
    public class AdminBatteryPageTestAdmin : CustomAuthenticatedPageTest
    {
        private const string baseSuffix = "/admin/battery/";

        private static readonly List<UserNameDto> users = [
            new () {
                Id = 5,
                FullName = "Barabich, Bas",
                FirstName = "Bas",
                FamilyName = "Barabich",
            },
            new () {
                Id = 4,
                FullName = "Chin, Bindo",
                FirstName = "Bindo",
                FamilyName = "Chin",
            },
            new () {
                Id = 9,
                FullName = "De Clerck, Kimberlie",
                FirstName = "Kimberlie",
                FamilyName = "De Clerck",
            },
            new () {
                Id = 2,
                FullName = "de Clerk, Bram",
                FirstName = "Bram",
                FamilyName = "de Clerk",
            },
            new () {
                Id = 6,
                FullName = "Helks, Pushwant",
                FirstName = "Pushwant",
                FamilyName = "Helks",
            },
            new () {
                Id = 1,
                FullName = "Her De Gaver, Patrick",
                FirstName = "Patrick",
                FamilyName = "Her De Gaver",
            },
            new () {
                Id = 7,
                FullName = "Montu, Sujan",
                FirstName = "Sujan",
                FamilyName = "Montu",
            },
            new () {
                Id = 3,
                FullName = "Piatti, Simon",
                FirstName = "Simon",
                FamilyName = "Piatti",
            },
            new () {
                Id = 8,
                FullName = "Serket, Xan",
                FirstName = "Xan",
                FamilyName = "Serket",
            },
        ];


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

        private static RouteFulfillOptions FulfillWithNotFoundResponse(RouteFulfillOptions fulfillOptions)
        {
            fulfillOptions.Status = (int)HttpStatusCode.NotFound;
            fulfillOptions.ContentType = "text/json";
            fulfillOptions.Body = JsonSerializer.Serialize("Object not found");
            return fulfillOptions;
        }

        private static RouteFulfillOptions FulfillWithBadRequestResponse(RouteFulfillOptions fulfillOptions)
        {
            fulfillOptions.Status = (int)HttpStatusCode.BadRequest;
            fulfillOptions.ContentType = "text/json";
            fulfillOptions.Body = JsonSerializer.Serialize("Bad Id");
            return fulfillOptions;
        }

        private async Task MockUserNames(int? delayMs = null)
        {
            await Page.RouteAsync($"**/api/user/names**", async (route) =>
            {
                if (delayMs.HasValue)
                {
                    await Task.Delay(delayMs.Value);
                }
                RouteFulfillOptions fulfillOptions = new();

                Uri uri = new(route.Request.Url);
                var queryParams = HttpUtility.ParseQueryString(uri.Query);
                string partialName = queryParams["partialName"]?.Trim() ?? "";

                fulfillOptions = FulfillWithOkResponse(fulfillOptions, users.Where(u => u.FullName.ToLower().Contains(partialName.ToLower())));
            });
        }

        private async Task MockBatteryDetailsValid(int batteryId, BatteryDto batteryDto, int? delayMs = null)
        {
            await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
            {
                if (delayMs.HasValue)
                {
                    await Task.Delay(delayMs.Value);
                }
                RouteFulfillOptions fulfillOptions = new();
                IRequest request = route.Request;
                if (request.Method == "GET")
                {
                    fulfillOptions = FulfillWithOkResponse(fulfillOptions, batteryDto);
                }
                else if (request.Method == "PUT")
                {
                    JsonElement? body = request.PostDataJSON();
                    UserNameDto userName = users[0];
                    BatteryDto newBattery = new()
                    {
                        Id = batteryId,
                        Type = body?.GetProperty("type").GetString() ?? "",
                        Mentor = new()
                        {
                            Id = body?.GetProperty("mentor").GetProperty("id").GetInt32() ?? 1,
                            FirstName = userName.FirstName,
                            FamilyName = userName.FamilyName,
                            FullName = userName.FullName,
                        },
                    };

                    fulfillOptions = FulfillWithOkResponse(fulfillOptions, newBattery);
                }

                await route.FulfillAsync(fulfillOptions);
            });
        }

        // ! don't have time to figure out how to test this
        // [Test]
        // public async Task ShouldUpdateBatteryDetails()
        // {
        //     int batteryId = 1;
        //     BatteryDto battery = new()
        //     {
        //         Id = batteryId,
        //         Type = "Lithium",
        //         Mentor = new()
        //         {
        //             Id = 1,
        //             FirstName = "John",
        //             FamilyName = "Madden",
        //             FullName = "Madden, John",
        //         }
        //     };
        //     await MockBatteryDetailsValid(batteryId, battery, 5000);
        //     await MockUserNames();

        //     await NavigateToUrl(baseSuffix);

        //     ILocator placeholder = Page.GetByTestId("battery-details-placeholder");
        //     await Expect(placeholder).ToBeVisibleAsync();
        //     ILocator submitButton = Page.GetByTestId("battery-details-submit");
        //     await Expect(submitButton).ToBeDisabledAsync();

        //     await Page.WaitForSelectorAsync("[data-testid=battery-details-placeholder]", new PageWaitForSelectorOptions() { State = WaitForSelectorState.Detached, Timeout = 0 });

        //     // Check current data
        //     // ******************

        //     ILocator type = Page.GetByTestId("battery-details-type");
        //     await Expect(type).ToBeVisibleAsync();
        //     await Expect(type).ToHaveValueAsync(battery.Type);

        //     ILocator mentorId = Page.GetByTestId("battery-details-mentor-id");
        //     await Expect(mentorId).ToBeVisibleAsync();
        //     await Expect(mentorId).ToHaveValueAsync(battery.Mentor.FullName);

        //     // Update data
        //     // ******************

        //     await mentorId.ClickAsync();
        //     UserNameDto userName = users[0];
        //     await mentorId.FillAsync(userName.FullName);

        //     await Expect(mentorId).ToHaveValueAsync(userName.FullName);
        //     var texts = await Page.GetByText(userName.FullName).AllAsync();
        //     await texts[^0].ClickAsync();

        //     await type.ClickAsync();
        //     await submitButton.ClickAsync();
        //     await Expect(placeholder).ToBeVisibleAsync();

        //     await Page.WaitForSelectorAsync("[data-testid=battery-details-placeholder]", new PageWaitForSelectorOptions() { State = WaitForSelectorState.Detached, Timeout = 0 });

        //     // Verify result
        //     // ************
        //     await Expect(type).ToHaveValueAsync(battery.Type);

        //     await Expect(mentorId).ToHaveValueAsync(userName.FullName);

        //     ILocator error = Page.GetByTestId("battery-details-fetch-error");
        //     await Expect(error).Not.ToBeVisibleAsync();

        //     ILocator success = Page.GetByTestId("battery-details-success");
        //     await Expect(success).ToBeVisibleAsync();
        // }

        [Test]
        public async Task ShouldNotFindBatteryId()
        {
            int batteryId = 666;
            await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
            {
                RouteFulfillOptions fulfillOptions = new();
                IRequest request = route.Request;
                if (request.Method == "GET")
                {
                    fulfillOptions = FulfillWithNotFoundResponse(fulfillOptions);
                }

                await route.FulfillAsync(fulfillOptions);
            });

            await NavigateToUrl(baseSuffix);

            ILocator error = Page.GetByTestId("battery-details-fetch-error");
            await Expect(error).ToBeVisibleAsync();
        }

        [Test]
        public async Task ShouldNotAllowBadBatteryId()
        {
            int batteryId = -1;
            await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
            {
                RouteFulfillOptions fulfillOptions = new();
                IRequest request = route.Request;
                if (request.Method == "GET")
                {
                    fulfillOptions = FulfillWithBadRequestResponse(fulfillOptions);
                }

                await route.FulfillAsync(fulfillOptions);
            });

            await NavigateToUrl(baseSuffix);

            ILocator error = Page.GetByTestId("battery-details-fetch-error");
            await Expect(error).ToBeVisibleAsync();
        }

        // [Test]
        // public async Task ShouldNotFindMentorId()
        // {
        //     int batteryId = 1;
        //     BatteryDto battery = new() { Id = batteryId, Type = "Lithium", MentorId = 1 };
        //     BatteryUpdateDto updatedBattery = new() { Type = "Zink", MentorId = 666 };
        //     await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
        //     {
        //         RouteFulfillOptions fulfillOptions = new();
        //         IRequest request = route.Request;
        //         if (request.Method == "GET")
        //         {
        //             fulfillOptions = FulfillWithOkResponse(fulfillOptions, battery);
        //         }
        //         else if (request.Method == "PUT")
        //         {
        //             fulfillOptions = FulfillWithNotFoundResponse(fulfillOptions);
        //         }

        //         await route.FulfillAsync(fulfillOptions);
        //     });

        //     await NavigateToUrl(baseSuffix);

        //     ILocator type = Page.GetByTestId("battery-details-type");
        //     await Expect(type).ToBeVisibleAsync();
        //     await Expect(type).ToHaveValueAsync(battery.Type);

        //     ILocator mentorId = Page.GetByTestId("battery-details-mentor-id");
        //     await Expect(mentorId).ToBeVisibleAsync();
        //     await Expect(mentorId).ToHaveValueAsync(battery.MentorId.ToString());

        //     ILocator submitButton = Page.GetByTestId("battery-details-submit");
        //     await submitButton.ClickAsync();

        //     ILocator error = Page.GetByTestId("battery-details-fetch-error");
        //     await Expect(error).ToBeVisibleAsync();
        // }

        // [Test]
        // public async Task ShouldNotAllowBadMentorId()
        // {
        //     int batteryId = 1;
        //     BatteryDto battery = new() { Id = batteryId, Type = "Lithium", MentorId = 1 };
        //     BatteryUpdateDto updatedBattery = new() { Type = "Zink", MentorId = -1 };
        //     await Page.RouteAsync($"**/api/Battery/{batteryId}", async (route) =>
        //     {
        //         RouteFulfillOptions fulfillOptions = new();
        //         IRequest request = route.Request;
        //         if (request.Method == "GET")
        //         {
        //             fulfillOptions = FulfillWithOkResponse(fulfillOptions, battery);
        //         }
        //         else if (request.Method == "PUT")
        //         {
        //             fulfillOptions = FulfillWithBadRequestResponse(fulfillOptions);
        //         }

        //         await route.FulfillAsync(fulfillOptions);
        //     });

        //     await NavigateToUrl(baseSuffix);

        //     ILocator type = Page.GetByTestId("battery-details-type");
        //     await Expect(type).ToBeVisibleAsync();
        //     await Expect(type).ToHaveValueAsync(battery.Type);

        //     ILocator mentorId = Page.GetByTestId("battery-details-mentor-id");
        //     await Expect(mentorId).ToBeVisibleAsync();
        //     await Expect(mentorId).ToHaveValueAsync(battery.MentorId.ToString());

        //     ILocator submitButton = Page.GetByTestId("battery-details-submit");
        //     await submitButton.ClickAsync();

        //     ILocator error = Page.GetByTestId("battery-details-fetch-error");
        //     await Expect(error).ToBeVisibleAsync();
        // }
    }
}
