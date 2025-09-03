using Microsoft.AspNetCore.Components;
using MudBlazor;
using MudBlazor.Utilities;
using Rise.Client.Reservations.Components.Dialogs;
using Rise.Shared.TimeSlots;
using Rise.Shared.Users;

namespace Rise.Client.Reservations.Components.TimeSlotList
{
    public partial class TimeSlotComponent
    {
        [Inject]
        public required IDialogService DialogService { get; set; }
        [Parameter]
        public required TimeSlotDto TimeSlot { get; set; }
        [Parameter]
        public DateOnly Date { get; set; }
        [Parameter, EditorRequired]
        public required Func<Task> RefetchData { get; set; }
        [Parameter, EditorRequired]
        public required UserProfileDto UserProfileDto { get; set; }

        private string StackStyle { get; set; } = GetStyle(AvailabilityEnum.Unavailable).style;
        private Color TextColor { get; set; } = GetColor(AvailabilityEnum.Unavailable);
        private string StackClassName { get; set; } = GetStyle(AvailabilityEnum.Unavailable).className;
        private string CursorClass { get; set; } = GetCursorClass(null);

        protected override void OnParametersSet()
        {
            var availability = TimeSlot.IsBookedByUser ? AvailabilityEnum.Booked : AvailabilityEnum.Available;
            StackStyle = GetStyle(availability).style;
            StackClassName = GetStyle(availability).className;
            TextColor = GetColor(availability);
            CursorClass = GetCursorClass(TimeSlot);
        }

        public static Color GetColor(AvailabilityEnum availability)
        {
            return availability switch
            {
                AvailabilityEnum.Available => Color.Default,
                AvailabilityEnum.Unavailable => Color.Error,
                AvailabilityEnum.Booked => Color.Primary,
                _ => Color.Default
            };
        }

        public static (string style, string className) GetStyle(AvailabilityEnum availability)
        {
            var baseStyle = new StyleBuilder()
                .AddStyle("background-color", availability switch
                {
                    AvailabilityEnum.Available => "rgba(var(--mud-palette-dark-rgb), 0.1)",
                    AvailabilityEnum.Unavailable => "rgba(var(--mud-palette-error-rgb), 0.1)",
                    AvailabilityEnum.Booked => "rgba(var(--mud-palette-primary-rgb), 0.1)",
                    _ => "rgba(var(--mud-palette-grey-rgb), 0.1)"
                })
                .Build();

            var className = availability switch
            {
                AvailabilityEnum.Available => "time-slot-available",
                AvailabilityEnum.Unavailable => "time-slot-unavailable",
                AvailabilityEnum.Booked => "time-slot-booked",
                _ => "time-slot-default"
            };

            return (baseStyle, className);
        }

        public async Task HandleClick()
        {
            if (TimeSlot.IsBookedByUser)
            {
                return;
            }

            var parameters = new DialogParameters<CreateReservationDialog> { { x => x.TimeSlot, TimeSlot }, { x => x.Date, Date }, { x => x.RefetchData, RefetchData }, { x => x.UserProfileDto, UserProfileDto } };
            var options = new DialogOptions { CloseButton = true };

            var dialog = await DialogService.ShowAsync<CreateReservationDialog>("Create reservation", parameters, options);
            var result = await dialog.Result;
        }

        private static string GetCursorClass(TimeSlotDto? TimeSlot)
        {
            if (TimeSlot is null)
            {
                return "cursor-default";
            }
            return TimeSlot.IsBookedByUser ? "cursor-default" : "cursor-pointer";
        }

        public enum AvailabilityEnum
        {
            Available = 0,
            Unavailable = 1,
            Booked = 2

        }
    }
}