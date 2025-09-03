using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Rise.Shared.Users;

namespace Rise.Client.Profile.Components;

public partial class LeftProfilePanel
{
    [Parameter]
    public required UserProfileDto UserProfileDto {get; set;}

    [Parameter]
    public required AuthenticationState AuthContext {get; set;}
}
