using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Boats;
using Rise.Domain.Users;

namespace Rise.Persistence.Boats
{
    /// <summary>
    /// Specific configuration for <see cref="Battery"/>.
    /// </summary>
    internal class BatteryConfiguration : EntityConfiguration<Battery>
    {
        public override void Configure(EntityTypeBuilder<Battery> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.Type).HasMaxLength(64);
            builder.Property(x => x.UsageCount)
                .HasDefaultValue(0);

            builder.HasOne(x => x.Boat)
            .WithMany(b => b.Batteries)
            .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Mentor)
            .WithMany(u => u.GuardedBatteries)
            .OnDelete(DeleteBehavior.SetNull);
        }
    }
}