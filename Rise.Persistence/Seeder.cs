using Rise.Persistence.Seeders;
using Rise.Persistence.Seeders.Boats;
using Rise.Persistence.Seeders.CruisePeriods;
using Rise.Persistence.Seeders.Notifications;
using Rise.Persistence.Seeders.Reservations;
using Rise.Persistence.Seeders.Users;

namespace Rise.Persistence;

/// <summary>
/// Seeder for filling the database
/// </summary>
/// <param name="dbContext">database context</param>
public class Seeder(ApplicationDbContext dbContext)
{

    /// <summary>
    /// Seed the application with entities
    /// </summary>
    public void Seed()
    {
        UserSeeder userSeeder = new(dbContext);
        userSeeder.Seed();

        BoatSeeder boatSeeder = new(dbContext);
        boatSeeder.Seed();
        BatterySeeder batterySeeder = new(dbContext, boatSeeder, userSeeder);
        batterySeeder.Seed();

        CruisePeriodSeeder cruisePeriodSeeder = new(dbContext);
        cruisePeriodSeeder.Seed();
        TimeSlotSeeder timeSlotSeeder = new(dbContext, cruisePeriodSeeder);
        timeSlotSeeder.Seed();

        ReservationSeeder reservationSeeder = new(dbContext, timeSlotSeeder, boatSeeder, userSeeder);
        reservationSeeder.Seed();

        NotificationSeeder notificationSeeder = new(dbContext, userSeeder);
        notificationSeeder.Seed();
    }
}