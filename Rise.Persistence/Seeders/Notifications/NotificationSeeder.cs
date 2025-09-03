using Microsoft.EntityFrameworkCore;
using Rise.Domain.Notifications;
using Rise.Persistence.Seeders.Users;

namespace Rise.Persistence.Seeders.Notifications
{
    internal class NotificationSeeder(ApplicationDbContext dbContext, UserSeeder userSeeder) : GeneralSeeder<Notification>(dbContext)
    {
        public readonly List<Notification> notifications =
            [
                new()
                {
                    Severity = 1, // Success
                    Title = "Success",
                    Message = "This data comes from the seeding of the db.",
                    CreatedAt = DateTime.Now,
                    IsRead = false,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 0, // Info
                    Title = "Info",
                    Message = "This is an info message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(5)),
                    IsRead = false,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 2, // Warning
                    Title = "Warning",
                    Message = "This is a warning message, but it is really long so it will be truncated",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(10)),
                    IsRead = false,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 3, // Error
                    Title = "Error",
                    Message = "This is an error message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(5))),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 3, // Error
                    Title = "Error",
                    Message = "This is an error message",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1).Add(TimeSpan.FromMinutes(6))),
                    IsRead = false,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 1, // Success
                    Title = "Deployment Complete",
                    Message = "Application successfully deployed to production environment",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 0, // Info
                    Title = "System Update",
                    Message = "Scheduled maintenance will occur tomorrow at 2 AM",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2).Add(TimeSpan.FromHours(3))),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 2, // Warning
                    Title = "Storage Alert",
                    Message = "Server storage capacity reaching 80%, consider cleanup",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 1, // Success
                    Title = "Backup Complete",
                    Message = "Weekly backup completed successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(4)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 0, // Info
                    Title = "New Feature",
                    Message = "Dark mode is now available in your settings",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(5)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 3, // Error
                    Title = "Connection Failed",
                    Message = "Unable to connect to secondary database server",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(5).Add(TimeSpan.FromHours(6))),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 2, // Warning
                    Title = "CPU Usage High",
                    Message = "System CPU usage exceeded 90% for 5 minutes",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(6)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 1, // Success
                    Title = "Test Suite Passed",
                    Message = "All integration tests completed successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(7)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 0, // Info
                    Title = "Profile Updated",
                    Message = "Your profile information has been updated successfully",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(8)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 2, // Warning
                    Title = "SSL Certificate",
                    Message = "SSL Certificate will expire in 30 days",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(9)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 3, // Error
                    Title = "Payment Failed",
                    Message = "Monthly subscription payment processing failed",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(10)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 1, // Success
                    Title = "Report Generated",
                    Message = "Monthly analytics report has been generated",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(11)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 0, // Info
                    Title = "Team Meeting",
                    Message = "Reminder: Team meeting scheduled for tomorrow at 10 AM",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(12)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 2, // Warning
                    Title = "API Rate Limit",
                    Message = "API rate limit reached 85% of maximum allocation",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(13)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 3, // Error
                    Title = "Security Alert",
                    Message = "Multiple failed login attempts detected from unknown IP",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(14)),
                    IsRead = true,
                    User = userSeeder.users[0]
                },
                new()
                {
                    Severity = 1, // Success
                    Title = "Welcome!",
                    Message = "Welcome to our platform! Your account has been successfully created.",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromHours(1)),
                    IsRead = false,
                    User = userSeeder.users[1]
                },
                new()
                {
                    Severity = 0, // Info
                    Title = "Getting Started Guide",
                    Message = "Check out our getting started guide to make the most of your account.",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromHours(2)),
                    IsRead = false,
                    User = userSeeder.users[1]
                },
                new()
                {
                    Severity = 2, // Warning
                    Title = "Complete Your Profile",
                    Message = "Your profile is incomplete. Add more information to unlock all features.",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(1)),
                    IsRead = true,
                    User = userSeeder.users[1]
                },

                new()
                {
                    Severity = 3, // Error
                    Title = "Password Reset Required",
                    Message = "For security reasons, please reset your password immediately.",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromMinutes(30)),
                    IsRead = false,
                    User = userSeeder.users[2]
                },
                new()
                {
                    Severity = 1, // Success
                    Title = "Project Milestone Reached",
                    Message = "Congratulations! Your team has completed the first milestone.",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(2)),
                    IsRead = true,
                    User = userSeeder.users[2]
                },
                new()
                {
                    Severity = 0, // Info
                    Title = "Subscription Status",
                    Message = "Your premium subscription will renew in 7 days.",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromHours(12)),
                    IsRead = false,
                    User = userSeeder.users[3]
                },
                new()
                {
                    Severity = 2, // Warning
                    Title = "Storage Space Low",
                    Message = "Your personal storage space is almost full. Consider upgrading your plan.",
                    CreatedAt = DateTime.Now.Subtract(TimeSpan.FromDays(3)),
                    IsRead = true,
                    User = userSeeder.users[3]
                }
            ];


        protected override DbSet<Notification> DbSet => _dbContext.Notifications;
        protected override IEnumerable<Notification> Items => notifications.AsEnumerable();
    }
}