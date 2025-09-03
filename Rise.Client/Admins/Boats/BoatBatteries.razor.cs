using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Boats;

namespace Rise.Client.Admins.Boats
{
    public partial class BoatBatteries : ComponentBase
    {
        [Parameter]
        public required int BoatId { get; set; }

        public required AsyncData<IEnumerable<BatteryDto>> AsyncDataRef { get; set; }
        private IEnumerable<BatteryDto> Batteries { get; set; } = Array.Empty<BatteryDto>();

        [Inject]
        public required IBatteryService BatteryService { get; set; }

        private async Task<IEnumerable<BatteryDto>> FetchBatteries()
        {
            return await BatteryService.GetBatteriesByBoat(BoatId);
        }
    }
}