using Microsoft.AspNetCore.Components;

namespace Rise.Client.Home
{
    public partial class Index : ComponentBase
    {
        protected override void OnInitialized()
        {
            navigation.NavigateTo("/home");
        }
    }
}