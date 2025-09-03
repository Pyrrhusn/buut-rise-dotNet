using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.TimeSlots;

namespace Rise.Persistence.TimeSlots;

internal class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
{
    public void Configure(EntityTypeBuilder<TimeSlot> builder)
    {
        builder.ToTable("TimeSlot");

        builder.Property(x => x.Date).IsRequired();
        builder.Property(x => x.Start).IsRequired();
        builder.Property(x => x.End).IsRequired();

        // Configure foreign key relationship
        builder.HasOne(x => x.CruisePeriod)
            .WithMany(x => x.TimeSlots)
            .OnDelete(DeleteBehavior.Cascade);
    }
}



