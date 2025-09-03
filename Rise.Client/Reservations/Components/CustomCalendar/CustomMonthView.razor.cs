using Microsoft.AspNetCore.Components;
using Heron.MudCalendar;
using MudBlazor.Utilities;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components.CustomCalendar
{
    public partial class CustomMonthView
    {
        [Parameter]
        public Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> AvailableDays { get; set; } = [];
        public DateTime Today { get; set; } = DateTime.Today;
        protected override string DayClassname(CalendarCell calendarCell)
        {
            return new CssBuilder(base.DayClassname(calendarCell))
            .AddClass("greyed-out-text", IsGreyedOut(calendarCell))
            .Build();
        }
        protected override string DayStyle(CalendarCell calendarCell, int index)
        {
            bool isToday = calendarCell.Date.Date.Equals(Today);
            bool isGreyedOut = IsGreyedOut(calendarCell);

            return new StyleBuilder()
            .AddStyle(base.DayStyle(calendarCell, index))
            .AddStyle("border", "var(--custom-today-border)", isToday)
            .AddStyle("background-color", "var(--custom-grey-out-color)", isGreyedOut)
            .AddStyle("color", "var(--mud-palette-text-disabled)", isGreyedOut)
            .Build();
        }

        private bool IsGreyedOut(CalendarCell calendarCell)
        {
            return !AvailableDays.ContainsKey(DateOnly.FromDateTime(calendarCell.Date));
        }

        private string GetCellTypes(CalendarCell calendarCell)
        {
            if (IsGreyedOut(calendarCell))
                return "fully-booked";

            return "available";
        }
    }
}


