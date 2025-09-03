using Microsoft.EntityFrameworkCore;
using Rise.Domain.TimeSlots;

namespace Rise.Persistence.Seeders.CruisePeriods
{
    internal class TimeSlotSeeder(ApplicationDbContext dbContext, CruisePeriodSeeder cruisePeriodSeeder) : GeneralSeeder<TimeSlot>(dbContext)
    {
        /// <summary>
        /// Time slots per day for cruise period that started one month ago and ends until two days before today.
        /// </summary>
        /// <see cref="CruisePeriodSeeder.PastMonthLong"/>
        public readonly CruiseSchedule PastMonthLong = new CruiseSchedule(cruisePeriodSeeder.PastMonthLong)
        .FillPeriod()
        ;


        /// <summary>
        /// Time slots per day for cruise period that starts now for a full week.
        /// </summary>
        /// <see cref="CruisePeriodSeeder.WeekLong"/>
        public readonly CruiseSchedule WeekLong =
        new CruiseSchedule(cruisePeriodSeeder.WeekLong)
        .MakePresetWeek();

        /// <summary>
        /// Time slots per day for cruise period that starts nine days from now for two weeks and 2 days. 
        /// </summary>
        /// <see cref="CruisePeriodSeeder.TwoWeekAndTwoDaysLong"/>
        public readonly CruiseSchedule TwoWeekAndTwoDaysLong = new CruiseSchedule(cruisePeriodSeeder.TwoWeekAndTwoDaysLong)
        .MakePresetWeek()
        // Start + 1 week + 5 day(s)
        .WithCruiseDay(12)
            .AddTimeSlot(new(10, 0, 0), new(13, 0, 0))
            .AddTimeSlot(new(14, 0, 0), new(17, 0, 0))
            .AddTimeSlot(new(18, 0, 0), new(21, 0, 0))
            .Done()
        // Start + 1 week + 6 day(s)
        .WithCruiseDay(13)
            .AddTimeSlot(new(10, 0, 0), new(13, 0, 0))
            .AddTimeSlot(new(14, 0, 0), new(17, 0, 0))
            .AddTimeSlot(new(18, 0, 0), new(21, 0, 0))
            .Done()
        // Start + 1 week + 8 day(s)
        .WithCruiseDay(15)
            .AddTimeSlot(new(9, 0, 0), new(12, 0, 0))
            .AddTimeSlot(new(14, 0, 0), new(16, 0, 0))
            .AddTimeSlot(new(19, 0, 0), new(22, 0, 0))
            .Done()
        ;

        /// <summary>
        /// Time slots per day for cruise period that starts next month from now for a month. 
        /// </summary>
        /// <see cref="CruisePeriodSeeder.MonthLong"/>
        public readonly CruiseSchedule MonthLong = new CruiseSchedule(cruisePeriodSeeder.MonthLong)
        .FillPeriod()
        ;

        protected override DbSet<TimeSlot> DbSet => _dbContext.TimeSlots;

        protected override IEnumerable<TimeSlot> Items => new List<CruiseSchedule>() { PastMonthLong, WeekLong, TwoWeekAndTwoDaysLong, MonthLong }.SelectMany(x => x.ToTimeSlots());
    }
}