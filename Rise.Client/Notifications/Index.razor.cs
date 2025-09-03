using Microsoft.AspNetCore.Components;
using Rise.Client.Common;
using Rise.Shared.Notifications;

namespace Rise.Client.Notifications
{
    public partial class Index
    {
        [Parameter]
        public int? NotificationId { get; set; }
        [Inject]
        public required NavigationManager NavigationManager { get; set; }
        [Inject]
        public required INotificationService NotificationService { get; set; }

        private NotificationDto? SelectedNotification { get; set; }
        private IEnumerable<NotificationDto> Notifications { get; set; } = [];

        public required AsyncData<IEnumerable<NotificationDto>> AsyncDataRef { get; set; }

        private void HandleNotificationSelected(NotificationDto notification)
        {
            NavigationManager.NavigateTo($"/notifications/{notification.Id}");
        }

        private Task<IEnumerable<NotificationDto>> FetchNotifications()
        {
            return NotificationService.GetUserNotifications();
        }

        // This method is called when the navigation parameter is set.
        protected override void OnParametersSet()
        {
            SelectNotificationById();
        }
        private void SelectNotificationById()
        {
            if (NotificationId.HasValue && Notifications.Any())
            {
                var notification = Notifications.FirstOrDefault(n => n.Id == NotificationId);
                if (notification != null)
                {
                    SelectedNotification = notification;
                    if (!notification.IsRead)
                    {
                        notification.IsRead = true;
                        try
                        {
                            NotificationService.MarkNotificationAsRead(notification.Id);
                        }
                        catch (Exception)
                        {
                            notification.IsRead = false;
                        }
                    }
                }
            }

        }
    }
}
