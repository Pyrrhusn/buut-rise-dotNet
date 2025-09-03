using Microsoft.AspNetCore.Components;
using MudBlazor;
using Microsoft.Extensions.Localization;
using Rise.Client.Localization.Reservations;

namespace Rise.Client.Reservations.Components.Dialogs
{
    public partial class CancelReservationDialog
    {
        [CascadingParameter] 
        MudDialogInstance MudDialog { get; set; } = default!;

        [Inject]
        private IStringLocalizer<ReservationPageResources> Localizer { get; set; } = default!;

        private void Submit()
        {
            MudDialog.Close(DialogResult.Ok(true));
        }

        private void Cancel()
        {
            MudDialog.Cancel();
        }
    }
} 