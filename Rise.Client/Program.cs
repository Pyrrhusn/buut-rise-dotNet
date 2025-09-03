using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.AspNetCore.Components.Web;
using Rise.Client;
using MudBlazor.Services;
using Rise.Shared.TimeSlots;
using Rise.Shared.Reservations;
using Rise.Client.TimeSlots;
using Rise.Client.Services;
using MudBlazor;
using System.Globalization;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Serilog.Core;
using Serilog;
using Rise.Shared.Notifications;
using Rise.Client.Notifications;
using Rise.Shared.Users;
using Rise.Client.Admins;
using Rise.Client.Localization.Register;
using Rise.Client.Auth;
using Rise.Shared.Boats;
using Rise.Client.Admins.Battery;
using Rise.Client.Admins.CruisePeriods;
using Rise.Client.Register;
using Rise.Client.Profile;
using Rise.Shared;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

var levelSwitch = new LoggingLevelSwitch();
Log.Logger = new LoggerConfiguration().MinimumLevel.ControlledBy(levelSwitch)
                .Enrich.WithProperty("InstanceId", Guid.NewGuid().ToString("n"))
                .WriteTo.BrowserConsole()
                .WriteTo.BrowserHttp($"{builder.HostEnvironment.BaseAddress}ingest", controlLevelSwitch: levelSwitch)
                .CreateLogger();

builder.Services.AddLocalization();
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddMudServices(config =>
{
    config.SnackbarConfiguration.PositionClass = Defaults.Classes.Position.BottomRight;
    config.SnackbarConfiguration.NewestOnTop = false;
    config.SnackbarConfiguration.VisibleStateDuration = 8000;
    config.SnackbarConfiguration.HideTransitionDuration = 500;
    config.SnackbarConfiguration.ShowTransitionDuration = 100;

});
builder.Services.AddMudPopoverService();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddOidcAuthentication(options =>
{
    builder.Configuration.Bind("Auth0", options.ProviderOptions);
    options.ProviderOptions.ResponseType = "code";
    options.ProviderOptions.PostLogoutRedirectUri = builder.HostEnvironment.BaseAddress;
    options.ProviderOptions.AdditionalProviderParameters.Add("audience", builder.Configuration["Auth0:Audience"]!);
}).AddAccountClaimsPrincipalFactory<ArrayClaimsPrincipalFactory<RemoteUserAccount>>(
);

builder.Services.AddHttpClient<ITimeSlotService, TimeSlotService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/TimeSlot/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IReservationService, ReservationService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Reservation/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<INotificationService, NotificationService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Notification/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IUserAdminService, UserAdminService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/User/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IReservationService, ReservationService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Reservation/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();


builder.Services.AddHttpClient<IUserRegisterService, UserRegisterService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/User/register");
});

builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/User/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<ICruisePeriodService, CruisePeriodService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/CruisePeriod/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IBatteryService, BatteryService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Battery/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

builder.Services.AddHttpClient<IBoatService, BoatService>(client =>
{
    client.BaseAddress = new Uri($"{builder.HostEnvironment.BaseAddress}api/Boat/");
}).AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

var host = builder.Build();

const string defaultCulture = "nl-BE";

var js = host.Services.GetRequiredService<IJSRuntime>();
var result = await js.InvokeAsync<string>("blazorCulture.get");
var culture = CultureInfo.GetCultureInfo(result ?? defaultCulture);

if (result == null)
{
    await js.InvokeVoidAsync("blazorCulture.set", defaultCulture);
}

CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

Log.Information("Starting up Client in Environment: {Environment}",
                builder.HostEnvironment.Environment);

await host.RunAsync();
