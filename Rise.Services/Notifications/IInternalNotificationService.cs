using Rise.Domain.Reservations;
using Rise.Domain.Users;

namespace Rise.Shared.Notifications;

public interface IInternalNotificationService
{
    Task SendNotificationToUser(int userId, string title, string message, SeverityEnum severity);

    Task SendNotificationToUser(User user, string title, string message, SeverityEnum severity);

    Task SendBatteryNotificationToUser(Reservation reservation);

    Task SendBatteryNotificationsToUsers(List<Reservation> reservations);

}
