using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Rise.Client.Common;
using Rise.Shared.Users;
using static Rise.Shared.Users.UpdateUserProfileDto;

namespace Rise.Client.Profile;


public partial class Index : ComponentBase
{
    [Inject]
    public required IUserService UserService { get; set; }

    public required AsyncData<UserProfileDto> AsyncDataRef { get; set; }
    private UserProfileDto? UserProfileDto { get; set; }

    private async Task<UserProfileDto> FetchUserProfile()
    {
        return await UserService.GetUserProfile();
    }

    private void HandleUserChanged(UserProfileDto updatedUser)
    {
        UserProfileDto = updatedUser;
    }
}
