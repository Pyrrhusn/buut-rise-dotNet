
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Rise.Client.Layout
{

    public partial class AdminLayout : LayoutComponentBase
    {
        private bool _drawerOpen = false;

        private void ToggleDrawer()
        {
            _drawerOpen = !_drawerOpen;
        }
    }


}