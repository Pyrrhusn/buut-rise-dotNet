using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Reservations;

namespace Rise.Persistence.Reservations
{/// <summary>
 /// Specific configuration for <see cref="Reservation"/>.
 /// </summary>
    internal class ReservationConfiguration : EntityConfiguration<Reservation>
    {
        public override void Configure(EntityTypeBuilder<Reservation> builder)
        {
            base.Configure(builder);

            builder
                .HasOne(e => e.Battery)
                .WithMany(e => e.Reservations)
                .IsRequired(false);

            builder
                .HasOne(e => e.Boat)
                .WithMany(e => e.Reservations)
                .HasForeignKey(e => e.BoatId)
                .IsRequired(true);

            builder
                .HasOne(e => e.TimeSlot)
                .WithMany(e => e.Reservations)
                .HasForeignKey(e => e.TimeSlotId)
                .IsRequired(true);

            builder
                .HasOne(e => e.User)
                .WithMany(e => e.Reservations)
                .HasForeignKey(e => e.UserId)
                .IsRequired(true);

            //boat has one reservation for each timeslot
            builder
                .HasIndex(e => new { e.BoatId, e.TimeSlotId })
                .IsUnique()
                .HasDatabaseName("IX_Unique_Boat_TimeSlot");

            //user has one reservation for each timeslot 
            builder
                .HasIndex(e => new { e.UserId, e.TimeSlotId })
                .IsUnique()
                .HasDatabaseName("IX_Unique_User_TimeSlot");

            builder
                .HasOne(e => e.PreviousBatteryHolder)
                .WithMany(e => e.HoldsBatteries)
                .IsRequired(false);
        }
    }
}