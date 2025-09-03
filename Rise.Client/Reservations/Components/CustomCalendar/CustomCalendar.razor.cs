using System.Globalization;
using Heron.MudCalendar;
using Heron.MudCalendar.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor.Extensions;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations.Components.CustomCalendar
{
    public partial class CustomCalendar
    {

        [SupplyParameterFromQuery]
        /// <summary>
        /// The actual start date for the date range
        /// </summary>
        private DateOnly? CurrentDate { get; set; }
        private DateTime? PickerDate
        {
            get => CurrentDay;
            set => CurrentDay = value ?? DateTime.Today;
        }

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private IJSRuntime JsRuntime { get; set; } = default!;


        private JsService? _jsService;

        private CalendarDateRange? _currentDateRange;

        private CalendarDatePicker? _datePicker;

        [Parameter]
        public Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> AvailableDays { get; set; } = [];

        [Parameter]
        public bool AllowPast { get; set; } = false;


        protected override void OnInitialized()
        {
            if (CurrentDate.HasValue && (AllowPast || CurrentDate.Value.CompareTo(DateOnly.FromDateTime(DateTime.Today)) >= 0))
            {
                DateTime CurrentDateTime = CurrentDate.Value.ToDateTime(TimeOnly.MinValue);
                CurrentDay = CurrentDateTime;
            }
            else
            {
                base.OnInitialized();
                NavigateToCurrentDate();
            }
        }


        /// <summary>
        /// Method from original component: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L473
        /// </summary>
        /// <param name="firstRender"></param>
        /// <returns></returns>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(false);

            if (firstRender)
            {

                await ChangeDateRange();

                await SetLinks();
            }
        }

        protected override async Task OnPreviousClicked()
        {
            if (CanGoToPreviousRange())
            {
                await base.OnPreviousClicked();
            }
        }

        /// <summary>
        /// If the view can go back to the previous range 
        /// </summary>
        /// <returns>If the view can go back to the previous range</returns>
        private bool CanGoToPreviousRange()
        {
            if (AllowPast)
            {
                return true;
            }

            DateTime newDate = View switch
            {
                CalendarView.Day => CurrentDay.AddDays(-1),
                CalendarView.Week => CurrentDay.AddDays(-7),
                CalendarView.WorkWeek => CurrentDay.AddDays(-7),
                CalendarView.Month => CurrentDay.AddMonths(-1),
                _ => CurrentDay
            };

            return IsBeforeCurrentMonth(newDate);
        }

        /// <summary>
        /// If the given date is before today's month
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private static bool IsBeforeCurrentMonth(DateTime date)
        {
            return date.CompareTo(DateTime.Today.StartOfMonth(CultureInfo.CurrentCulture)) >= 0;
        }

        /// <summary>
        /// Method from original component: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L599
        /// </summary>
        /// <param name="dateTime">The selected date picker</param>
        /// <returns></returns>
        private async Task DatePickerDateChanged(DateTime? dateTime)
        {
            var dateChanged = dateTime.HasValue && dateTime != CurrentDay;

            PickerDate = dateTime;

            if (dateChanged) await CurrentDayChanged.InvokeAsync(CurrentDay);

            await ChangeDateRange(new CalendarDateRange(dateTime ?? DateTime.Today, View, GetFirstDayOfWeekByCalendarView(View)));
        }

        /// <summary>
        /// Method from original component: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L615
        /// </summary>
        /// <returns></returns>
        private async Task ChangeDateRange()
        {
            await ChangeDateRange(new CalendarDateRange(CurrentDay, View, GetFirstDayOfWeekByCalendarView(View)));
        }

        /// <summary>
        /// Method from original component since it's private: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L620
        /// </summary>
        /// <param name="dateRange">The selected date range</param>
        /// <returns></returns>
        private async Task ChangeDateRange(CalendarDateRange dateRange)
        {
            if (dateRange != _currentDateRange)
            {
                _currentDateRange = dateRange;
                NavigateToCurrentDate();
                await DateRangeChanged.InvokeAsync(dateRange);
            }
        }

        /// <summary>
        /// Method from original component since it's private: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L610
        /// </summary>
        private void OnDatePickerOpened()
        {
            _datePicker?.GoToDate(CurrentDay);
        }

        /// <summary>
        /// Method from original component since it's private: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L629
        /// </summary>
        /// <returns>Allowed views</returns>
        private List<CalendarView> AllowedViews()
        {
            var list = new List<CalendarView>();
            if (ShowDay) list.Add(CalendarView.Day);
            if (ShowWeek) list.Add(CalendarView.Week);
            if (ShowWorkWeek) list.Add(CalendarView.WorkWeek);
            if (ShowMonth) list.Add(CalendarView.Month);
            return list;
        }

        /// <summary>
        /// Method from original component: https://github.com/danheron/Heron.MudCalendar/blob/a7a9698c68453407b90ad06787241e3446d92fb9/Heron.MudCalendar/Components/MudCalendar.razor.cs#L588
        /// </summary>
        /// <returns></returns>
        private async Task SetLinks()
        {
            // Check if link is already set
            _jsService ??= new JsService(JsRuntime);
            var head = await _jsService.GetHeadContent();
            if (!string.IsNullOrEmpty(head) && head.Contains("Heron.MudCalendar.min.css")) return;

            // Add link
            await _jsService.AddLink("_content/Heron.MudCalendar/Heron.MudCalendar.min.css", "stylesheet");
        }

        /// <summary>
        /// Navigates in URL query the current date
        /// </summary>
        private void NavigateToCurrentDate()
        {
            Dictionary<string, object?> queries = new()
            {
                ["CurrentDate"] = CurrentDay.ToString("yyyy-MM-dd"),
            };
            Navigation.NavigateTo(Navigation.GetUriWithQueryParameters(queries), forceLoad: false, replace: true);
        }

    }
}


