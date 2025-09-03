using System;
using Rise.Domain.Notifications;
using Rise.Domain.Tests.TestUtilities;
using Shouldly;

namespace Rise.Domain.Tests.Notifications
{
    public class NotificationsShould
    {
        [Fact]
        public void BeCreatedWithValidProperties()
        {
            Notification notification = new NotificationBuilder().Build();
            notification.Severity.ShouldBe(NotificationBuilder.ValidSeverity);
            notification.Title.ShouldBe(NotificationBuilder.ValidTitle);
            notification.Message.ShouldBe(NotificationBuilder.ValidMessage);
            notification.IsRead.ShouldBe(NotificationBuilder.ValidIsRead);
            notification.User.ShouldBe(NotificationBuilder.ValidUser);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(4)]
        public void ThrowWhenSeverityOutOfRange(int severity)
        {
            Should.Throw<ArgumentOutOfRangeException>(() =>
                new NotificationBuilder()
                    .WithSeverity(severity)
                    .Build()
            );
        }

        [Fact]
        public void ThrowWhenTitleIsNull()
        {
            Should.Throw<ArgumentNullException>(() =>
                new NotificationBuilder()
                    .WithTitle(null!)
                    .Build()
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ThrowWhenTitleIsEmptyOrWhitespace(string title)
        {
            Should.Throw<ArgumentException>(() =>
                new NotificationBuilder()
                    .WithTitle(title)
                    .Build()
            );
        }

        [Fact]
        public void ThrowWhenMessageIsNull()
        {
            Should.Throw<ArgumentNullException>(() =>
                new NotificationBuilder()
                    .WithMessage(null!)
                    .Build()
            );
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void ThrowWhenMessageIsEmptyOrWhitespace(string message)
        {
            Should.Throw<ArgumentException>(() =>
                new NotificationBuilder()
                    .WithMessage(message)
                    .Build()
            );
        }

        [Fact]
        public void ThrowWhenUserIsNull()
        {
            Should.Throw<ArgumentException>(() =>
                new NotificationBuilder()
                    .WithUser(null!)
                    .Build()
            );
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        public void AllowValidSeverityValues(int severity)
        {
            var notification = new NotificationBuilder()
                .WithSeverity(severity)
                .Build();
            notification.Severity.ShouldBe(severity);
        }

        [Fact]
        public void AllowUpdatingIsReadStatus()
        {
            var notification = new NotificationBuilder()
                .WithIsRead(false)
                .Build();

            notification.IsRead = true;
            notification.IsRead.ShouldBeTrue();
        }

        [Fact]
        public void InitializeWithCurrentTimestamp()
        {
            var notification = new NotificationBuilder().Build();
            notification.CreatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
            notification.UpdatedAt.ShouldBeLessThanOrEqualTo(DateTime.UtcNow);
        }
    }
}