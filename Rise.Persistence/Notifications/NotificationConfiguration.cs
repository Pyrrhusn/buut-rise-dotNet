using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Notifications;

namespace Rise.Persistence.Notifications
{
    internal class NotificationConfiguration : EntityConfiguration<Notification>
    {
        public override void Configure(EntityTypeBuilder<Notification> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.Title)
                        .HasMaxLength(130);

            builder
                .HasOne(e => e.User)
                .WithMany(e => e.Notifications)
                .IsRequired(true);
        }
    }
}