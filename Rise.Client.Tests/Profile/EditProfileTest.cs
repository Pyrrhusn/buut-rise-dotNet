using Microsoft.Playwright;
using Rise.Shared.Address;
using Rise.Shared.Users;

namespace Rise.Client.Tests.Profile;

public class EditProfileTest : CustomAuthenticatedPageTest
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
        await MockProfileApi();
        await NavigateToUrl(ProfilePageUrl);
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

    private async Task MockPatchProfileApi(int delayMs = 0)
    {
        await Page.RouteAsync($"*/**/api/User/", async route =>
        {

            if (delayMs > 0)
                await Task.Delay(delayMs);

            await route.FulfillAsync(new()
            {
                Status = 204,
                ContentType = "application/json",
            });
        });
    }

    [Test]
    public async Task ClickEditButtonShowsEditableUserProfile()
    {
        var edit = Page.GetByTestId("profile-edit-button");
        await edit.ClickAsync();

        await Expect(edit).ToBeHiddenAsync();

        var save = Page.GetByTestId("profile-save-button");
        var cancel = Page.GetByTestId("profile-cancel-button");

        await Expect(save).ToBeVisibleAsync();
        await Expect(cancel).ToBeVisibleAsync();

        var firstName = Page.GetByTestId("profile-first-name");
        await Expect(firstName).ToHaveValueAsync($"{profileDto.FirstName}");
        await Expect(firstName).ToBeEnabledAsync();

        var familyName = Page.GetByTestId("profile-family-name");
        await Expect(familyName).ToHaveValueAsync($"{profileDto.FamilyName}");
        await Expect(familyName).ToBeEnabledAsync();

        var phoneNumber = Page.GetByTestId("profile-phone-number");
        await Expect(phoneNumber).ToHaveValueAsync($"{profileDto.PhoneNumber}");
        await Expect(phoneNumber).ToBeEnabledAsync();

        var street = Page.GetByTestId("profile-street");
        await Expect(street).ToHaveValueAsync($"{profileDto.Address.Street}");
        await Expect(street).ToBeEnabledAsync();

        var number = Page.GetByTestId("profile-number");
        await Expect(number).ToHaveValueAsync($"{profileDto.Address.Number}");
        await Expect(number).ToBeEnabledAsync();

        var city = Page.GetByTestId("profile-city");
        await Expect(city).ToHaveValueAsync($"{profileDto.Address.City}");
        await Expect(city).ToBeEnabledAsync();

        var postalCode = Page.GetByTestId("profile-postal-code");
        await Expect(postalCode).ToHaveValueAsync($"{profileDto.Address.PostalCode}");
        await Expect(postalCode).ToBeEnabledAsync();

        var country = Page.GetByTestId("profile-country");
        await Expect(country).ToHaveValueAsync($"{profileDto.Address.Country}");
        await Expect(country).ToBeEnabledAsync();
    }

    [Test]
    public async Task ClickSaveButtonChangesFirstName()
    {
        await MockPatchProfileApi();

        var edit = Page.GetByTestId("profile-edit-button");
        await edit.ClickAsync();

        var save = Page.GetByTestId("profile-save-button");

        await Expect(save).ToBeVisibleAsync();

        var firstName = Page.GetByTestId("profile-first-name");
        await Expect(firstName).ToHaveValueAsync($"{profileDto.FirstName}");
        await Expect(firstName).ToBeEnabledAsync();

        await firstName.ClickAsync();

        await Expect(firstName).ToBeFocusedAsync();

        await firstName.FillAsync("NewName");

        await save.ClickAsync();

        await Page.WaitForSelectorAsync("[data-testid='profile-save-loading']", new() { State = WaitForSelectorState.Hidden });

        await Expect(firstName).ToBeDisabledAsync();
        await Expect(firstName).ToHaveValueAsync("NewName");
        await Expect(Page.GetByTestId("profile-full-name")).ToHaveTextAsync($"NewName {profileDto.FamilyName}");
    }
}
