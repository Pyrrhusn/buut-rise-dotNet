using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;

namespace Rise.Client.Layout
{
    public partial class NavMenu : IDisposable
    {
        private bool _drawerOpen = false;
        private bool _notificationPopoverOpen = false;
        private bool _isAuthenticated = false;
        private AuthenticationStateChangedHandler? _authHandler;

        [Inject]
        public required NavigationManager Navigation { get; set; }
        [Inject]
        public required AuthenticationStateProvider AuthStateProvider { get; set; }

        protected override void OnInitialized()
        {
            _authHandler = async (Task<AuthenticationState> task) =>
            {
                var authState = await task;
                _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
                await InvokeAsync(StateHasChanged);
            };

            AuthStateProvider.AuthenticationStateChanged += _authHandler;
        }

        public void Dispose()
        {
            if (_authHandler != null)
            {
                AuthStateProvider.AuthenticationStateChanged -= _authHandler;
            }
        }

        protected override async Task OnInitializedAsync()
        {
            var authState = await AuthStateProvider.GetAuthenticationStateAsync();
            _isAuthenticated = authState.User.Identity?.IsAuthenticated ?? false;
        }

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }

        private void HandleNotificationButtonClicked()
        {
            _notificationPopoverOpen = !_notificationPopoverOpen;
        }

        public void BeginLogOut()
        {
            Navigation.NavigateToLogout("authentication/logout");
        }
    }
}