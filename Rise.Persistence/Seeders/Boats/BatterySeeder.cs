using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Persistence.Seeders.Users;

namespace Rise.Persistence.Seeders.Boats
{
    internal class BatterySeeder(ApplicationDbContext dbContext, BoatSeeder boatSeeder, UserSeeder userSeeder) : GeneralSeeder<Battery>(dbContext)
    {

        /// <summary>
        /// Batteries for Limba
        /// </summary>
        /// <see cref="BoatSeeder.Limba"/>
        public readonly BoatBatteryCollection LimbaBatteries = new BoatBatteryCollection(boatSeeder.Limba)
        .AddBattery(type: "Lithium-Ion", mentor: userSeeder.users[0])
        .AddBattery(type: "Loodzuur", mentor: userSeeder.users[1])
        .AddBattery(type: "NiMH", mentor: userSeeder.users[2])
        ;

        /// <summary>
        /// Batteries for Leith
        /// </summary>
        /// <see cref="BoatSeeder.Leith"/>
        public readonly BoatBatteryCollection LeithBatteries = new BoatBatteryCollection(boatSeeder.Leith)
        .AddBattery(type: "Lithium-Ion", mentor: userSeeder.users[3])
        .AddBattery(type: "Loodzuur", mentor: userSeeder.users[4])
        .AddBattery(type: "NiMH", mentor: userSeeder.users[5])
        ;

        /// <summary>
        /// Batteries for Lubeck
        /// </summary>
        /// <see cref="BoatSeeder.Lubeck"/>
        public readonly BoatBatteryCollection LubeckBatteries = new BoatBatteryCollection(boatSeeder.Lubeck)
        .AddBattery(type: "Lithium-Ion", mentor: userSeeder.users[6])
        .AddBattery(type: "Loodzuur", mentor: userSeeder.users[7])
        .AddBattery(type: "NiMH", mentor: userSeeder.users[8])
        ;
        protected override DbSet<Battery> DbSet => _dbContext.Batteries;

        protected override IEnumerable<Battery> Items => new List<BoatBatteryCollection>() {
            LimbaBatteries, LeithBatteries, LubeckBatteries
        }.SelectMany(b => b.batteries);

    }
}
