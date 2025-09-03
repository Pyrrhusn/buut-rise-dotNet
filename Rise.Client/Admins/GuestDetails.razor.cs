using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using MudBlazor;
using Rise.Client.Common;
using Rise.Client.Localization.Admins;
using Rise.Shared.Users;


namespace Rise.Client.Admins
{
    public partial class GuestDetails : ComponentBase
    {
        [Parameter]
        public int Id { get; set; }

        public required AsyncData<UserDetailDto> AsyncDataRef { get; set; }
        private UserDetailDto? UserDetails { get; set; }

        private string FormattedFullName => UserDetails != null
            ? $"{UserDetails.FirstName} {UserDetails.FamilyName}"
            : string.Empty;
        private string FormattedAddress => UserDetails?.Address != null
            ? $"{UserDetails.Address.Street} {UserDetails.Address.Number}, {UserDetails.Address.PostalCode} {UserDetails.Address.City}, {UserDetails.Address.Country}"
            : string.Empty;


        [Inject]
        public required IUserAdminService UserService { get; set; }

        [Inject]
        public required NavigationManager NavigationManager { get; set; }

        [Inject]
        public required ISnackbar Snackbar { get; set; }

        [Inject]
        public required IStringLocalizer<AdminPageResources> AdminPageLocalizer { get; set; }

        private bool isProcessing = false;

        protected void NavigateToListPage()
        {

            NavigationManager.NavigateTo("/admin/guests");

        }

        private async Task ApproveGuest()
        {
            try
            {
                isProcessing = true;
                StateHasChanged();

                await UserService.AddMemberRole(Id);

                Snackbar.Add(AdminPageLocalizer["ApproveSuccess"], Severity.Success);

                NavigateToListPage();
            }
            catch (Exception ex)
            {
                Snackbar.Add(string.Format(AdminPageLocalizer["ApproveFailure"], ex.Message), Severity.Error);
            }
            finally
            {
                isProcessing = false;
                StateHasChanged();
            }
        }

        private Task<UserDetailDto> FetchUserDetails()
        {
            return UserService.GetUserDetails(Id);
        }
    }
}