using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Reservations;
using Rise.Shared.TimeSlots;
using Rise.Shared.Users;

namespace Rise.Client.Reservations.Components.Dialogs
{
    public partial class CreateReservationDialog
    {
        [CascadingParameter]
        private MudDialogInstance MudDialog { get; set; } = default!;
        [Parameter]
        public DateOnly Date { get; set; }
        [Parameter]
        public required TimeSlotDto TimeSlot { get; set; }
        [Parameter]
        public required UserProfileDto UserProfileDto { get; set; }
        [Inject]
        private IReservationService ReservationService { get; set; } = default!;

        [Parameter, EditorRequired]
        public required Func<Task> RefetchData { get; set; }

        private DialogState State { get; set; } = DialogState.Overview;

        private void Close() => MudDialog.Close();
        private void Cancel() => MudDialog.Cancel();

        private async void CreateReservation()
        {
            State = DialogState.Pay;
            await Task.Delay(3500);
            await ReservationService.CreateReservation(new CreateReservationDto
            {
                TimeSlotId = TimeSlot.Id
            });
            State = DialogState.Success;
            await RefetchData();
            StateHasChanged();
        }
        private enum DialogState
        {
            Overview,
            Pay,
            Success
        }
    }
}
