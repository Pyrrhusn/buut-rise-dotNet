using System;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Rise.Client.Auth;

public partial class RedirectToLogin
{
    [Inject] protected NavigationManager Navigation { get; set; } = default!;

    protected override void OnInitialized()
    {
        Navigation.NavigateToLogin("authentication/login");
    }
}
