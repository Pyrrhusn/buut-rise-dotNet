using Rise.Domain.TimeSlots;
using Rise.Persistence.Seeders.Boats;
using Rise.Persistence.Seeders.CruisePeriods;
using Rise.Persistence.Seeders.Users;

namespace Rise.Persistence.Seeders.Reservations
{
    internal class ReservationPeriodSchedule(CruiseSchedule periodSchedule)
    {
        internal readonly CruiseSchedule periodSchedule = periodSchedule;

        public ReservationDay WithReservationDay(int daysFromStart)
        {
            return WithReservationDay(DateOnly.FromDateTime(periodSchedule.cruisePeriod.Start.AddDays(daysFromStart)));
        }

        public ReservationDay WithReservationDay(DateOnly date)
        {
            ReservationDay reservationDaySchedule = new(this, periodSchedule.GetCruiseDay(date));
            return reservationDaySchedule;
        }

        public ReservationPeriodSchedule FillPeriod(BoatSeeder boatSeeder, UserSeeder userSeeder)
        {
            CruisePeriod period = periodSchedule.cruisePeriod;

            period.TimeSlots.GroupBy(t => t.Date, t => t.Start)
            .Select(g => new
            {
                Date = g.Key,
                MinStart = g.Min()
            }).ToList()
            .ForEach(
                item =>
                {
                    WithReservationDay(item.Date)
                        .WithTimeSlot(0)
                            .AddReservation(boat: boatSeeder.Limba, user: userSeeder.users[0])
                            .AddReservation(boat: boatSeeder.Leith, user: userSeeder.users[1])
                        .Done();
                }
            );

            return this;
        }
    }
}