using System;
using Microsoft.Playwright;
using Rise.Shared.Address;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Profile;

public class ProfilePageTest : CustomAuthenticatedPageTest
{
    private const string ProfilePageUrl = "/profile";

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

    [SetUp]
    public async Task SetUp()
    {
        await LoginAsync(UserRole.Guest);
    }

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

    private async Task MockProfileApiError()
    {
        await Page.RouteAsync($"*/**/api/User/profile**", async route =>
        {
            await route.FulfillAsync(new() { Status = 404, Body = "Not found" });
        });
    }

    [Test]
    public async Task ShowsLoadingStateWhileFetchingProfile()
    {
        await MockProfileApi(5000);

        await NavigateToUrl(ProfilePageUrl);

        await Expect(Page.GetByTestId("profile-loading-progress")).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShowsErrorStateWhenApiReturns404()
    {
        await MockProfileApiError();
        await NavigateToUrl(ProfilePageUrl);

        await Page.WaitForSelectorAsync("[data-testid='profile-loading-progress']", new() { State = WaitForSelectorState.Hidden });
        await Expect(Page.GetByTestId("profile-fetch-error")).ToBeVisibleAsync();
        await Page.WaitForSelectorAsync("[data-testid='profile-fetch-error']", new() { State = WaitForSelectorState.Visible });

        ILocator errorMessage = Page.GetByTestId("profile-fetch-error");
        await Expect(errorMessage).ToBeVisibleAsync();
    }

    [Test]
    public async Task ShowsUserProfile()
    {
        await MockProfileApi();
        await NavigateToUrl(ProfilePageUrl);

        await Expect(Page.GetByTestId("profile-full-name")).ToHaveTextAsync($"{profileDto.FirstName} {profileDto.FamilyName}");
        await Expect(Page.GetByTestId("profile-date-of-birth")).ToHaveTextAsync($"{profileDto.DateOfBirth:dd/MM/yyyy}");
        await Expect(Page.GetByTestId("profile-roles")).ToHaveTextAsync($"{nameof(UserRole.Guest)}");

        var firstName = Page.GetByTestId("profile-first-name");
        await Expect(firstName).ToHaveValueAsync($"{profileDto.FirstName}");
        await Expect(firstName).ToBeDisabledAsync();

        var family = Page.GetByTestId("profile-family-name");
        await Expect(family).ToHaveValueAsync($"{profileDto.FamilyName}");
        await Expect(family).ToBeDisabledAsync();

        var phone = Page.GetByTestId("profile-phone-number");
        await Expect(phone).ToHaveValueAsync($"{profileDto.PhoneNumber}");
        await Expect(phone).ToBeDisabledAsync();

        var street = Page.GetByTestId("profile-street");
        await Expect(street).ToHaveValueAsync($"{profileDto.Address.Street}");
        await Expect(street).ToBeDisabledAsync();

        var number = Page.GetByTestId("profile-number");
        await Expect(number).ToHaveValueAsync($"{profileDto.Address.Number}");
        await Expect(number).ToBeDisabledAsync();

        var city = Page.GetByTestId("profile-city");
        await Expect(city).ToHaveValueAsync($"{profileDto.Address.City}");
        await Expect(city).ToBeDisabledAsync();

        var postal = Page.GetByTestId("profile-postal-code");
        await Expect(postal).ToHaveValueAsync($"{profileDto.Address.PostalCode}");
        await Expect(postal).ToBeDisabledAsync();

        var country = Page.GetByTestId("profile-country");
        await Expect(country).ToHaveValueAsync($"{profileDto.Address.Country}");
        await Expect(country).ToBeDisabledAsync();

        var edit = Page.GetByTestId("profile-edit-button");
        await Expect(edit).ToBeVisibleAsync();

        var save = Page.GetByTestId("profile-save-button");
        var cancel = Page.GetByTestId("profile-cancel-button");

        await Expect(save).ToBeHiddenAsync();
        await Expect(cancel).ToBeHiddenAsync();
    }

}
