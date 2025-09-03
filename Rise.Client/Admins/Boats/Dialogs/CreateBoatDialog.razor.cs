using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared;
using Rise.Shared.Boats;
using static Rise.Shared.Boats.CreateBoatDto;

namespace Rise.Client.Admins.Boats.Dialogs;

public partial class CreateBoatDialog : ComponentBase
{
    [Inject]
    public required IBoatService BoatService { get; set; }

    [Inject]
    public required ISnackbar SnackbarService { get; set; }

    [CascadingParameter]
    private MudDialogInstance MudDialog { get; set; } = default!;

    private CreateBoatDto Boat = new()
    {
        PersonalName = string.Empty,
        IsAvailable = default,
    };

    private MudForm Form = default!;

    private Validator Validator = new();

    private bool IsLoading = false;

    private void Close() => MudDialog.Close();

    private async Task SaveBoat()
    {
        IsLoading = true;

        await Form.Validate();

        if (Form.IsValid)
        {
            try
            {
                await BoatService.CreateBoatAsync(Boat);
            }
            catch (Exception e)
            {
                SnackbarService.Add($"Sonething went wrong: {e.Message}", Severity.Error);
                IsLoading = false;
                return;
            }

            SnackbarService.Add("Successfully created a new boat", Severity.Success);
            StateHasChanged();
            Close();
        }
        IsLoading = false;
    }
}
