using Heron.MudCalendar;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Reservations
{
    public partial class Index : ComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public bool Past { get; set; }

        // TODO refactor tab tracer to tabs components
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            ChangeTabViaName();
        }
        private static readonly List<string> tabNames = ["calendar", "reservations"];

        private int _tabIndex = 0;
        private int TabIndex
        {
            get => _tabIndex; set
            {
                UpdateSelectedTabInQuery(value);
                _tabIndex = value;
            }
        }

        [SupplyParameterFromQuery]
        /// <summary>
        /// Which tab is open on the page
        /// </summary>
        private string? CurrentTab { get; set; }

        private void UpdateSelectedTabInQuery(int index)
        {
            string tabName = tabNames[index];
            bool noCurrentTabName = CurrentTab is null;
            if (noCurrentTabName || (CurrentTab is not null && !CurrentTab.Equals(tabName)))
            {
                Dictionary<string, object?> queries = new()
                {
                    ["CurrentTab"] = tabName,
                };
                Navigation.NavigateTo(Navigation.GetUriWithQueryParameters(queries), forceLoad: false, replace: true);
            }

        }

        private void ChangeTabViaName()
        {
            if (CurrentTab is not null)
            {
                // TODO fix so that incase missing tabs doesn't crash (ex. guests can brows to this page and crash it)
                CurrentTab = CurrentTab.ToLower();
                int index = tabNames.IndexOf(CurrentTab);
                TabIndex = index < 0 ? 0 : index;
            }
            else
            {
                UpdateSelectedTabInQuery(TabIndex);
            }
        }

    }

    public partial class ColoredCalendarItem : CalendarItem
    {
        public Color Color { get; set; } = Color.Primary;
    }
}