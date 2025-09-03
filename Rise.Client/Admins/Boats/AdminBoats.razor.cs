using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Client.Admins.Boats.Dialogs;
using Rise.Client.Common;
using Rise.Shared;
using Serilog;

namespace Rise.Client.Admins.Boats
{
    public partial class AdminBoats : ComponentBase
    {
        private List<BoatDto>? boats;

        public required AsyncData<List<BoatDto>> AsyncDataRef { get; set; }

        [Inject]
        public required IBoatService boatService { get; set; }

        [Inject]
        public required ISnackbar snackbar { get; set; }

        [Inject]
        public required IDialogService DialogService { get; set; }

        private async Task<List<BoatDto>?> LoadBoats()
        {
            return (await boatService.GetAllBoatsAsync()).ToList();
        }

        private async Task UpdateBoatAvailabilityAsync(int boatId, bool isAvailable)
        {
            try
            {
                Console.WriteLine($"Updating boat {boatId} to {isAvailable}");
                await boatService.UpdateBoatAvailabilityAsync(boatId, isAvailable);
                snackbar.Add($"Boat availability updated to {(isAvailable ? "Available" : "Unavailable")}.", Severity.Success);
                boats = await LoadBoats();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating boat: {ex}");
                snackbar.Add($"Error updating boat availability: {ex.Message}", Severity.Error);
            }
        }

        private void EditBoat(int boatId)
        {
            // TODO: Implement EditBoat logic
        }

        private void DeleteBoat(int boatId)
        {
            // TODO: Implement DeleteBoat logic
        }

        private async Task HandleAddBoat()
        {
            var result = await ShowAddBoatDialog();
            if (result?.Canceled == false)
            {
                boats = await LoadBoats();
                StateHasChanged();
            }
        }

        private async Task<DialogResult?> ShowAddBoatDialog()
        {
            var parameters = new DialogParameters<CreateBoatDialog> { };
            var options = new DialogOptions { CloseButton = true };

            var dialog = await DialogService.ShowAsync<CreateBoatDialog>("Create boat", parameters, options);
            return await dialog.Result;
        }
    }
}
