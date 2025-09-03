using Microsoft.AspNetCore.Components;
using Rise.Shared.TimeSlots;
using Rise.Shared.Users;

namespace Rise.Client.Reservations.Components.Dialogs.DialogContent
{
    public partial class CreateReservationOverviewContent
    {
        [Parameter] public required UserProfileDto UserProfileDto { get; set; }
        [Parameter] public required DateOnly Date { get; set; }
        [Parameter] public required TimeSlotDto TimeSlot { get; set; }
    }
}