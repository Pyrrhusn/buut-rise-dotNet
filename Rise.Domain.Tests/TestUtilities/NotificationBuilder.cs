using Rise.Domain.Notifications;
using Rise.Domain.Users;

namespace Rise.Domain.Tests.TestUtilities
{
    public class NotificationBuilder
    {
        public static readonly int ValidSeverity = 1;
        public static readonly string ValidTitle = "Notice";
        public static readonly string ValidMessage = "Hello, World!";
        public static readonly bool ValidIsRead = false;
        public static readonly User ValidUser = new UserBuilder().Build();
        public static readonly int ValidUserId = ValidUser.Id;

        private int severity = ValidSeverity;
        private string title = ValidTitle;
        private string message = ValidMessage;
        private bool isRead = ValidIsRead;
        private User user = ValidUser;

        public NotificationBuilder WithSeverity(int severity)
        {
            this.severity = severity;
            return this;
        }

        public NotificationBuilder WithTitle(string title)
        {
            this.title = title;
            return this;
        }

        public NotificationBuilder WithMessage(string message)
        {
            this.message = message;
            return this;
        }

        public NotificationBuilder WithIsRead(bool isRead)
        {
            this.isRead = isRead;
            return this;
        }

        public NotificationBuilder WithUser(User user)
        {
            this.user = user;
            return this;
        }

        public Notification Build()
        {
            return new Notification
            {
                Severity = severity,
                Title = title,
                Message = message,
                IsRead = isRead,
                User = user
            };
        }
    }
}