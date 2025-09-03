namespace Rise.Client.Register;

using Microsoft.AspNetCore.Components;
using Serilog;
using Rise.Shared.Users;
using FluentValidation;
using FluentValidation.Results;
using MudBlazor;
using Microsoft.Extensions.Localization;
using Rise.Client.Localization.Register;
using static Rise.Shared.Users.UserRegistrationModelDto;

public partial class Index : ComponentBase
{
    [Inject]
    public required IStringLocalizer<RegisterFormPageResources> Localizer { get; set; }

    [Inject]
    public required ISnackbar SnackbarService { get; set; }

    [Inject]
    public required IUserRegisterService UserRegisterService { get; set; }

    private UserRegistrationModelDto User = new()
    {
        FirstName = string.Empty,
        FamilyName = string.Empty,
        Email = string.Empty,
        Password = string.Empty,
        PhoneNumber = string.Empty,
        DateOfBirth = default,
        Address = new UserRegistrationModelDto.AddressModel
        {
            Street = string.Empty,
            Number = string.Empty,
            City = string.Empty,
            PostalCode = string.Empty,
            Country = "Belgium"
        }
    };

    private bool isLoading = false;
    private bool isSuccess;

    private MudForm Form = null!;
    private Validator validator = new();

    private string CheckPasswordMatch(string passwordRepeat)
    {
        if (User.Password != passwordRepeat)
        {
            return "Passwords do not match";
        }
        return string.Empty;
    }

    private async Task HandleSubmit()
    {
        isLoading = true;
        await Form.Validate();

        try
        {
            if (Form.IsValid)
            {
                await UserRegisterService.RegisterUser(User);
                isSuccess = true;
                SnackbarService.Add("Successfully registered! Check your email for further instruction.", MudBlazor.Severity.Success);

                await Form.ResetAsync();
                User.Address.Country = "Belgium";
            }
            else
            {
                SnackbarService.Add("Please correct the errors before submitting the form.", MudBlazor.Severity.Error);
            }
        }
        catch (Exception ex)
        {
            Log.Error($"Error registering user:\n{ex.Message}");
            SnackbarService.Add($"An error occured while trying to register:\n{ex.Message}", MudBlazor.Severity.Error);
        }
        finally
        {
            isLoading = false;
        }
    }

    private bool isPasswordShow;
    private InputType PasswordInputType = InputType.Password;
    private string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

    private void TogglePasswordIcon()
    {
        if (isPasswordShow)
        {
            isPasswordShow = false;
            PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
            PasswordInputType = InputType.Password;
        }
        else
        {
            isPasswordShow = true;
            PasswordInputIcon = Icons.Material.Filled.Visibility;
            PasswordInputType = InputType.Text;
        }
    }
}
