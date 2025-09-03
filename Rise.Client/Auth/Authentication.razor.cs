using System;
using Microsoft.AspNetCore.Components;

namespace Rise.Client.Auth;

public partial class Authentication
{
    [Parameter] public string? Action { get; set; }
}
