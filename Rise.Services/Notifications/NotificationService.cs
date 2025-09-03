using Ardalis.GuardClauses;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Notifications;
using Rise.Persistence;
using Rise.Services.Auth;
using Rise.Shared.Notifications;

namespace Rise.Services.Notifications
{

    public class NotificationService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
        : AuthenticatedService(dbContext, authContextProvider), INotificationService
    {
        public async Task<IEnumerable<NotificationDto>> GetUserNotifications(int? limit)
        {
            int userId = (int)_authContextProvider.GetUserId()!;

            IEnumerable<NotificationDto> notifications = (await _dbContext.Users.Include(user => user.Notifications)
                    .FirstAsync(user => user.Id == userId))
                .Notifications.OrderByDescending(notification => notification.CreatedAt)
                .Select(MapNotificationToDto);

            if (limit.HasValue)
            {
                notifications = notifications.Take(limit.Value);
            }

            return notifications.ToList();
        }

        public Task MarkNotificationAsRead(int id)
        {
            int userId = (int)_authContextProvider.GetUserId()!;

            Notification? notification = _dbContext.Users.Include(user => user.Notifications)
                .First(user => user.Id == userId)
                .Notifications
                .FirstOrDefault(notification => notification.Id == id) ?? throw new NotFoundException(id.ToString(), typeof(Notification).ToString());

            notification.IsRead = true;

            return _dbContext.SaveChangesAsync();
        }

        private static NotificationDto MapNotificationToDto(Notification notification)
        {
            return new NotificationDto
            {
                Id = notification.Id,
                Severity = (SeverityEnum)notification.Severity,
                Title = notification.Title,
                Message = notification.Message,
                CreatedAt = notification.CreatedAt,
                IsRead = notification.IsRead
            };
        }

        public async Task<int> GetUnreadNotificationCount()
        {
            int userId = (int)_authContextProvider.GetUserId()!;

            return (await _dbContext.Users.Include(user => user.Notifications)
                    .FirstAsync(user => user.Id == userId))
                .Notifications.Where(notification => !notification.IsRead).Count();
        }
    }

}
