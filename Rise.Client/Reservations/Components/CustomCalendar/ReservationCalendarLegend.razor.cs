using Microsoft.AspNetCore.Components;

namespace Rise.Client.Reservations.Components.CustomCalendar;

public partial class ReservationCalendarLegend
{
    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object>? AdditionalAttributes { get; set; }
}
