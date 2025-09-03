using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Shared.Boats;
using Rise.Shared.Users;
using Serilog;

namespace Rise.Client.Admins.Battery
{
    public partial class BatteryDetailsPage
    {
        private static BatteryUpdateDto DefaultBatteryUpdateDto =>
        new()
        {
            MentorId = -1,
            Type = "Lithium",
        };

        private static UserNameDto DefaultUserNameDto =>
        new()
        {
            Id = -1,
            FirstName = "first name",
            FamilyName = "last name",
            FullName = "full name",
        };

        [Inject]
        public required ISnackbar Snackbar { get; set; }

        [Inject]
        public required IBatteryService BatteryService { get; set; }

        [Inject]
        public required IUserAdminService UserService { get; set; }

        [Parameter]
        public int? Id { get; set; }

        private BatteryUpdateDto batteryModel = DefaultBatteryUpdateDto;

        public required BatteryDto batteryInfo;
        public required UserNameDto mentor = DefaultUserNameDto;

        private readonly BatteryUpdateDto.Validator batteryValidator = new();

        public static BatteryUpdateDto BatteryToUpdateBattery(BatteryDto batteryDto)
        {
            return batteryDto is null ? DefaultBatteryUpdateDto : new()
            {
                MentorId = batteryDto.Mentor.Id,
                Type = batteryDto.Type,
            };
        }

        private Task<BatteryDto> FetchBatteryInfo()
        {
            return BatteryService.GetBattery(Id ?? 1);
        }

        private async Task<BatteryDto> HandleSubmit(BatteryUpdateDto batteryDetails)
        {
            return await BatteryService.UpdateBattery(Id ?? 1, batteryDetails);
        }

        private async Task<IEnumerable<UserNameDto>> SearchUsers(string searchText, CancellationToken token)
        {

            if (searchText.Equals(batteryInfo.Mentor.FullName))
                return [batteryInfo.Mentor];

            IEnumerable<UserNameDto> paginationUserNames = await UserService.GetUsersByFullName(searchText, token);

            return paginationUserNames;
        }

        private static string UserNameDtoToString(UserNameDto user)
        {
            return user.FullName;
        }

        private void OnValueChangeBatteryInfo(BatteryDto battery)
        {
            batteryInfo = battery;
            mentor = battery.Mentor;

        }

        private void OnValueChangeMentor(UserNameDto user)
        {
            mentor = user;
            batteryModel.MentorId = user.Id;
        }

    }
}