using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Boats;

namespace Rise.Persistence.Boats
{
    /// <summary>
    /// Specific configuration for <see cref="Boat"/>.
    /// </summary>
    internal class BoatConfiguration : EntityConfiguration<Boat>
    {
        public override void Configure(EntityTypeBuilder<Boat> builder)
        {
            base.Configure(builder);
            builder.Property(x => x.PersonalName).HasMaxLength(64);

            builder.Property(x => x.IsAvailable)
            .IsRequired()
            .HasDefaultValue(true);

            
        }
    }
}