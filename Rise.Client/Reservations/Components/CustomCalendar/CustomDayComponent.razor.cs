using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rise.Client.Reservations.Components.CustomCalendar;

public partial class CustomDayComponent
{
    [Parameter]
    public required CalendarItem Context { get; set; }
}
