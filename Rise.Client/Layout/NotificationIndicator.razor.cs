using Microsoft.AspNetCore.Components;
using Rise.Shared.Notifications;
namespace Rise.Client.Layout
{
    public partial class NotificationIndicator : ComponentBase
    {
        [Inject]
        public required INotificationService NotificationService { get; set; }

        [Parameter]
        public bool IsMobile { get; set; }

        [Parameter]
        public EventCallback OnNotificationClick { get; set; }

        [Parameter]
        public bool IsAuthenticated { get; set; }

        private int _unreadCount = 0;

        protected override async Task OnParametersSetAsync()
        {
            if (IsAuthenticated)
            {
                await UpdateNotificationCount();
            }
        }

        private async Task UpdateNotificationCount()
        {
            _unreadCount = await NotificationService.GetUnreadNotificationCount();
        }
    }
}
