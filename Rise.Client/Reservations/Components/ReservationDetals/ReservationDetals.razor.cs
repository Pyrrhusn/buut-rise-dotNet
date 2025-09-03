using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Client.Common;
using Rise.Shared.Reservations;
using Serilog;
using Rise.Client.Reservations.Components.Dialogs;

namespace Rise.Client.Reservations.Components.ReservationDetals
{
    public partial class ReservationDetals : ComponentBase
    {
        public required AsyncData<ReservationDetailsDto> AsyncDataRef { get; set; }
        protected ReservationDetailsDto? ReservationDetails { get; set; }

        [Parameter]
        public int Id { get; set; }

        [Inject]
        public required ISnackbar SnackbarService { get; set; }

        [Inject]
        public required IReservationService ReservationService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required IDialogService DialogService { get; set; }

        private bool IsReservationInPast => ReservationDetails?.Date < DateOnly.FromDateTime(DateTime.Now);
        private bool CanCancelReservation => 
            ReservationDetails != null && 
            !ReservationDetails.IsDeleted && 
            !IsReservationInPast && 
            ReservationDetails.Date >= DateOnly.FromDateTime(DateTime.Now.AddDays(2));

        
        private  static string GetDisplayText(string? value) => string.IsNullOrEmpty(value) ? "\u00A0" : value;

        protected Task<ReservationDetailsDto> GetReservationDetails()
        {
            return ReservationService.GetReservationDetailsAsync(Id);
        }

        private async Task CancelReservation()
        {
            if (ReservationDetails is null)
                return;

            var dialog = await DialogService.ShowAsync<CancelReservationDialog>(Localizer["CancelReservationTitle"]);
            var result = await dialog.Result;

            if (result?.Canceled == false)
            {
                try
                {
                    await ReservationService.CancelReservationAsync(ReservationDetails.Id);
                    ReservationDetails.IsDeleted = true;
                    StateHasChanged();

                    NavigationManager.NavigateTo("/reservations?CurrentTab=reservations");
                }
                catch (Exception ex)
                {
                    Log.Error($"Error cancelling reservation: {ex.Message}");
                    var message = ex.Message switch
                    {
                        "AlreadyCancelled" => Localizer["AlreadyCancelled"],
                        "CancellationTooLate" => Localizer["CancellationTooLate"],
                        _ => Localizer["CancellationError"]
                    };
                    SnackbarService.Add(RenderErrorMessage(message), Severity.Error);
                }
            }
        }

        private bool HasCurrentHolderDetails()
        {
            return !string.IsNullOrWhiteSpace(ReservationDetails?.CurrentBatteryUserName) ||
                   !string.IsNullOrWhiteSpace(ReservationDetails?.CurrentHolderPhoneNumber) ||
                   !string.IsNullOrWhiteSpace(ReservationDetails?.CurrentHolderEmail) ||
                   !string.IsNullOrWhiteSpace(ReservationDetails?.CurrentHolderStreet) ||
                   !string.IsNullOrWhiteSpace(ReservationDetails?.CurrentHolderCity);
        }

      
    }
}


