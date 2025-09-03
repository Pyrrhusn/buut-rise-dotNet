using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.TimeSlots;

namespace Rise.Persistence.TimeSlots;

internal class CruisePeriodConfiguration : IEntityTypeConfiguration<CruisePeriod>
{



    public void Configure(EntityTypeBuilder<CruisePeriod> builder)
    {
        builder.ToTable("CruisePeriod");

        builder.Property(x => x.Start).IsRequired();
        builder.Property(x => x.End).IsRequired();
    }
}
