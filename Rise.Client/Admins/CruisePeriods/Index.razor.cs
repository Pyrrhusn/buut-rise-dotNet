using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.TimeSlots;

namespace Rise.Client.Admins.CruisePeriods
{

    public partial class Index
    {
        [Inject]
        public required ICruisePeriodService CruisePeriodService { get; set; }

        public List<CruisePeriodDetailedDto> CruisePeriods { get; set; } = new();

        private bool ShowPastPeriods { get; set; }
        private AsyncData<List<CruisePeriodDetailedDto>>? _asyncData;

        protected override void OnInitialized()
        {
            ShowPastPeriods = false; // Default to showing future periods
        }

        public async Task<List<CruisePeriodDetailedDto>> FetchCruisePeriods()
        {
            return await CruisePeriodService.GetCruisePeriods(!ShowPastPeriods);
        }

        private async Task OnToggleChanged(bool toggled)
        {
            ShowPastPeriods = toggled;
            if (_asyncData != null)
            {
                await _asyncData.FetchData();
            }
        }
    }
}