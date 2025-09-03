using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Users;
using static Rise.Shared.Users.UpdateUserProfileDto;

namespace Rise.Client.Profile.Components;

public partial class RightProfilePanel
{
    [Inject]
    public required ISnackbar SnackbarService { get; set; }
    [Inject]
    public required IUserService UserService { get; set; }
    private UpdateUserProfileDto UpdateUserProfileDto = new()
    {
        Address = new()
        {
            Street = string.Empty,
            Number = string.Empty,
            City = string.Empty,
            PostalCode = string.Empty,
            Country = "Belgium"
        },
        FamilyName = string.Empty,
        FirstName = string.Empty,
        PhoneNumber = string.Empty,
    };

    [Parameter]
    public required UserProfileDto InitialProfileDto { get; set; }

    [Parameter]
    public EventCallback<UserProfileDto> OnUserChanged { get; set; }

    private MudForm Form = null!;

    private readonly Validator Validator = new();

    private bool EditIsEnabled = false;

    private bool IsLoading = false;

    protected override void OnInitialized()
    {
        base.OnInitialized();

        SetUserProfileDtoToInitialProfileDto();
    }

    private async Task SaveChanges()
    {
        if (!FieldsHaveChanged())
        {
            ToggleEdit();
            return;
        }

        IsLoading = true;
        await Form.Validate();

        if (Form.IsValid && FieldsHaveChanged())
        {
            ToggleEdit();
            try
            {
                await UserService.UpdateUserAsync(UpdateUserProfileDto);
            }
            catch (Exception e)
            {
                SnackbarService.Add($"Sonething went wrong: {e.Message}", Severity.Error);
                IsLoading = false;
                ToggleEdit();
                return;
            }

            SetInitialProfileDtoStateToCurrentUserProfileDto();
            await OnUserChanged.InvokeAsync(InitialProfileDto);

            SnackbarService.Add("you successfully updated your profile!", Severity.Success);

            Form.ResetValidation();
        }
        IsLoading = false;
    }

    private void CancelEdit()
    {
        SetUserProfileDtoToInitialProfileDto();
        Form.ResetValidation();
        ToggleEdit();
    }

    private void ToggleEdit()
    {
        EditIsEnabled = !EditIsEnabled;
    }

    private void SetUserProfileDtoToInitialProfileDto()
    {
        UpdateUserProfileDto = new UpdateUserProfileDto
        {
            FirstName = InitialProfileDto.FirstName,
            FamilyName = InitialProfileDto.FamilyName,
            PhoneNumber = InitialProfileDto.PhoneNumber,
            Address = new()
            {
                Street = InitialProfileDto.Address.Street,
                Number = InitialProfileDto.Address.Number,
                City = InitialProfileDto.Address.City,
                PostalCode = InitialProfileDto.Address.PostalCode,
                Country = InitialProfileDto.Address.Country,
            }
        };
    }

    private void SetInitialProfileDtoStateToCurrentUserProfileDto()
    {
        InitialProfileDto = new UserProfileDto
        {
            FirstName = UpdateUserProfileDto.FirstName,
            FamilyName = UpdateUserProfileDto.FamilyName,
            PhoneNumber = UpdateUserProfileDto.PhoneNumber,
            Address = new()
            {
                Street = UpdateUserProfileDto.Address.Street,
                Number = UpdateUserProfileDto.Address.Number,
                City = UpdateUserProfileDto.Address.City,
                PostalCode = UpdateUserProfileDto.Address.PostalCode,
                Country = UpdateUserProfileDto.Address.Country,
            },
            DateOfBirth = InitialProfileDto.DateOfBirth,
            Email = InitialProfileDto.Email,
        };
    }

    private bool FieldsHaveChanged()
    {
        return !UpdateUserProfileDto.FirstName.Equals(InitialProfileDto.FirstName) ||
        !UpdateUserProfileDto.FamilyName.Equals(InitialProfileDto.FamilyName) ||
        !UpdateUserProfileDto.PhoneNumber.Equals(InitialProfileDto.PhoneNumber) ||
        !UpdateUserProfileDto.Address.Street.Equals(InitialProfileDto.Address.Street) ||
        !UpdateUserProfileDto.Address.Number.Equals(InitialProfileDto.Address.Number) ||
        !UpdateUserProfileDto.Address.City.Equals(InitialProfileDto.Address.City) ||
        !UpdateUserProfileDto.Address.PostalCode.Equals(InitialProfileDto.Address.PostalCode) ||
        !UpdateUserProfileDto.Address.Country.Equals(InitialProfileDto.Address.Country);
    }
}
