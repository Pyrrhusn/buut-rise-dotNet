using Ardalis.GuardClauses;
using Microsoft.Extensions.Logging;
using Rise.Domain.Notifications;
using Rise.Domain.Reservations;
using Rise.Domain.Users;
using Rise.Persistence;
using Rise.Shared.Notifications;

namespace Rise.Services.Notifications
{

    public class InternalNotificationService(ApplicationDbContext dbContext, ILogger<InternalNotificationService> logger)
        : IInternalNotificationService
    {

        private async Task<Notification> MakeNotificationForUser(int userId, string title, string message, SeverityEnum severity)
        {
            User user = await dbContext.Users.FindAsync(userId) ?? throw new NotFoundException(userId.ToString(), typeof(User).ToString());
            return MakeNotificationForUser(user, title, message, severity);
        }

        private static Notification MakeNotificationForUser(User user, string title, string message, SeverityEnum severity)
        {
            return new()
            {
                Title = title,
                Message = message,
                Severity = (int)severity,
                User = user
            };
        }

        public async Task SendNotificationToUser(int userId, string title, string message, SeverityEnum severity)
        {
            await SendNotificationToUser(await MakeNotificationForUser(userId, title, message, severity));
        }

        public async Task SendNotificationToUser(User user, string title, string message, SeverityEnum severity)
        {
            await SendNotificationToUser(MakeNotificationForUser(user, title, message, severity));
        }

        private async Task SendNotificationToUser(Notification notification)
        {
            try
            {
                logger.LogInformation("Sending notification to user {userId} with severity {severity} and title {title}", notification.User.Id, notification.Severity, notification.Title);

                dbContext.Notifications.Add(notification);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending notification", ex);
            }
        }

        private async Task SendNotificationsToUsers(List<Notification> notifications)
        {
            try
            {
                logger.LogInformation("Sending notifications {amount} to users", notifications.Count);

                dbContext.Notifications.AddRange(notifications);

                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error sending notification", ex);
            }
        }

        private static Notification MakeBatteryNotificationToUser(Reservation reservation)
        {
            return MakeNotificationForUser(
                    user: reservation.User,
                    title: "Battery Assigned",
                    message: $"A battery has been assigned to your reservation for {reservation.TimeSlot.Date:d} at {reservation.TimeSlot.Start:t}.",
                    severity: SeverityEnum.Info
                );
        }


        public async Task SendBatteryNotificationToUser(Reservation reservation)
        {
            Notification notification = MakeBatteryNotificationToUser(reservation);
            await SendNotificationToUser(notification);
        }

        public async Task SendBatteryNotificationsToUsers(List<Reservation> reservations)
        {
            List<Notification> notifications = reservations
            .OrderBy(r => r.TimeSlot.Date)
            .ThenBy(r => r.TimeSlot.Start)
            .Select(MakeBatteryNotificationToUser).ToList();
            await SendNotificationsToUsers(notifications);
        }

    }

}
