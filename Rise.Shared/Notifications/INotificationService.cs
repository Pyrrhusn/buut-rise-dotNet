namespace Rise.Shared.Notifications
{

    public interface INotificationService
    {
        Task<IEnumerable<NotificationDto>> GetUserNotifications(int? limit = null);
        Task MarkNotificationAsRead(int id);
        Task<int> GetUnreadNotificationCount();
    }

}
