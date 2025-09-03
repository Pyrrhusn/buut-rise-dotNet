using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Rise.Domain.Users;

namespace Rise.Persistence.Users
{
    /// <summary>
    /// Specific configuration for <see cref="User"/>.
    /// </summary>
    internal class UserConfiguration : EntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Email).HasMaxLength(69).IsRequired();
            builder.HasIndex(x => x.Email).IsUnique().HasDatabaseName("IX_Unique_User_Email");

            builder.Property(x => x.FirstName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.FamilyName).HasMaxLength(100).IsRequired();
            builder.Property(x => x.FullName)
            .HasComputedColumnSql($"\"{nameof(User.FamilyName)}\" || ', ' || \"{nameof(User.FirstName)}\"", stored: true)
            .HasMaxLength(202).IsRequired();
            builder.Property(x => x.PhoneNumber).HasMaxLength(100).IsRequired();
            builder.Property(x => x.DateOfBirth).IsRequired();

            builder.OwnsOne(x => x.Address, address =>
            {
                address.Property(a => a.Street).HasMaxLength(200).IsRequired();
                address.Property(a => a.Number).HasMaxLength(200).IsRequired();
                address.Property(a => a.City).HasMaxLength(200).IsRequired();
                address.Property(a => a.PostalCode).HasMaxLength(100).IsRequired();
                address.Property(a => a.Country).HasMaxLength(100).IsRequired();
            });
        }
    }
}