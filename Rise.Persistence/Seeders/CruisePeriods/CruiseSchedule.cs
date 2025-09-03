using Rise.Domain.TimeSlots;

namespace Rise.Persistence.Seeders.CruisePeriods
{
    /// <summary>
    /// All days in a cruise period
    /// </summary>
    /// <param name="cruisePeriod">cruise period to schedule</param>
    internal class CruiseSchedule(CruisePeriod cruisePeriod) : ITimedSchedule
    {
        private readonly List<CruiseDay> cruiseDays = [];
        internal readonly CruisePeriod cruisePeriod = cruisePeriod;

        /// <summary>
        /// Start for assigning for a day relative from the starting point.
        /// </summary>
        /// <param name="daysFromStart">days from start of period</param>
        /// <returns>Start cruise day</returns>
        public CruiseDay WithCruiseDay(int daysFromStart)
        {
            return WithCruiseDay(DateOnly.FromDateTime(cruisePeriod.Start.AddDays(daysFromStart)));
        }

        /// <summary>
        /// Start for assigning for a day.
        /// </summary>
        /// <param name="date">date</param>
        /// <returns>Start cruise day</returns>
        public CruiseDay WithCruiseDay(DateOnly date)
        {
            CruiseDay cruiseDaySchedule = new(this, date);
            cruiseDays.Add(cruiseDaySchedule);
            return cruiseDaySchedule;
        }

        /// <summary>
        /// Gets cruise day that has given day
        /// </summary>
        /// <param name="date">day</param>
        /// <returns>Matching cruise day</returns>
        public CruiseDay GetCruiseDay(DateOnly date)
        {
            return cruiseDays.First(x => x.date.Equals(date));
        }

        /// <returns>Flatted collection of time slots for cruise period</returns>
        public IEnumerable<TimeSlot> ToTimeSlots()
        {
            return cruiseDays.SelectMany(x => x.ToTimeSlots());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="i"></param>
        /// <returns></returns>
        public CruiseSchedule MakePresetWeek(int i = 0)
        {
            // Start + i + 0 day(s)
            return WithCruiseDay(i + 0)
                    .AddTimeSlot(new(10, 0, 0), new(13, 0, 0))
                    .AddTimeSlot(new(14, 0, 0), new(17, 0, 0))
                    .AddTimeSlot(new(18, 0, 0), new(21, 0, 0))
                    .Done()
                // Start + i + 1 day(s)
                .WithCruiseDay(i + 1)
                    .AddTimeSlot(new(10, 0, 0), new(13, 0, 0))
                    .AddTimeSlot(new(14, 0, 0), new(17, 0, 0))
                    .AddTimeSlot(new(18, 0, 0), new(21, 0, 0))
                    .Done()
                // Start + i + 2 day(s)
                .WithCruiseDay(i + 2)
                    .AddTimeSlot(new(9, 0, 0), new(12, 0, 0))
                    .AddTimeSlot(new(14, 0, 0), new(16, 0, 0))
                    .AddTimeSlot(new(19, 0, 0), new(22, 0, 0))
                    .Done()
                // Start + i + 3 day(s)
                .WithCruiseDay(i + 3)
                    .AddTimeSlot(new(10, 0, 0), new(11, 30, 0))
                    .AddTimeSlot(new(13, 0, 0), new(14, 0, 0))
                    .AddTimeSlot(new(16, 30, 0), new(18, 45, 0))
                    .Done()
                // Start + i + 4 day(s) - Empty day, no time slots
                // Start + i + 5 day(s)
                .WithCruiseDay(i + 5)
                    .AddTimeSlot(new(10, 0, 0), new(13, 0, 0))
                    .AddTimeSlot(new(14, 0, 0), new(17, 0, 0))
                    .AddTimeSlot(new(18, 0, 0), new(21, 0, 0))
                    .Done()
                // Start + i + 6 day(s)
                .WithCruiseDay(i + 6)
                    .AddTimeSlot(new(10, 0, 0), new(13, 0, 0))
                    .Done()
                // Start + i + 7 day(s)
                .WithCruiseDay(i + 7)
                    .AddTimeSlot(new(10, 0, 0), new(11, 30, 0))
                    .AddTimeSlot(new(13, 0, 0), new(14, 0, 0))
                    .AddTimeSlot(new(16, 30, 0), new(18, 45, 0))
                    .Done();
        }

        public CruiseSchedule FillPeriod()
        {
            double totalDays = cruisePeriod.End.Subtract(cruisePeriod.Start).TotalDays - 1;
            int weekLength = 7;
            int totalWeeks = (int)(totalDays / weekLength);

            //  Fill in weeks
            for (int i = 0; i < totalWeeks; i++)
            {
                MakePresetWeek(i * weekLength);
            }

            // Fill in remaining days
            for (int i = totalWeeks * weekLength; i < totalDays - 2; i++)
            {
                WithCruiseDay(i)
                    .AddTimeSlot(new(10, 0, 0), new(13, 0, 0));
            }

            return this;
        }
    }


}