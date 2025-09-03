using Microsoft.EntityFrameworkCore;
// using Rise.Domain.Boats;
using Rise.Domain.Users;
using Rise.Domain.Reservations;
using Rise.Domain.Boats;
using Rise.Domain.TimeSlots;
using Rise.Domain.Notifications;

namespace Rise.Persistence;

/// <inheritdoc />
public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<CruisePeriod> CruisePeriods => Set<CruisePeriod>();
    public DbSet<TimeSlot> TimeSlots => Set<TimeSlot>();

    public DbSet<Boat> Boats => Set<Boat>();

    public DbSet<Battery> Batteries => Set<Battery>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Notification> Notifications => Set<Notification>();


    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // All columns in the database have a maxlength of 4000.
        // in NVARACHAR 4000 is the maximum length that can be indexed by a database.
        // Some columns need more length, but these can be set on the configuration level for that Entity in particular.
        configurationBuilder.Properties<string>().HaveMaxLength(4_000);
        // All decimals columns should have 2 digits after the comma
        configurationBuilder.Properties<decimal>().HavePrecision(18, 2);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Applying all types of IEntityTypeConfiguration in the Persistence project.
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }

}

