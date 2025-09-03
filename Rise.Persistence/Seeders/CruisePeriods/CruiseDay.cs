using Rise.Domain.TimeSlots;

namespace Rise.Persistence.Seeders.CruisePeriods
{
    /// <summary>
    /// All time slots on a day within a cruise period
    /// </summary>
    /// <param name="periodSchedule">the schedule for the cruise period</param>
    /// <param name="date">date in the cruise period</param>
    internal class CruiseDay(CruiseSchedule periodSchedule, DateOnly date) : ITimedSchedule
    {
        private readonly List<TimeSlot> timeSlots = [];

        public DateOnly date = date;

        public CruisePeriod CruisePeriod => periodSchedule.cruisePeriod;

        /// <summary>
        /// Adds a time slot to the schedule
        /// </summary>
        /// <param name="start">start time</param>
        /// <param name="end">end time</param>
        public CruiseDay AddTimeSlot(TimeOnly start, TimeOnly end)
        {
            TimeSlot timeSlot = new()
            {
                CruisePeriod = CruisePeriod,
                Date = date,
                Start = start,
                End = end,
            };
            CruisePeriod.AddTimeSlot(
                timeSlot
            );

            timeSlots.Add(timeSlot);

            return this;
        }

        /// <summary>
        /// Signal done adding new time slots
        /// </summary>
        /// <returns>cruiser schedule</returns>
        public CruiseSchedule Done()
        {
            return periodSchedule;
        }

        /// <summary>
        /// Gets the time slot that starts with given start time
        /// </summary>
        /// <param name="start">start time of the time slot</param>
        /// <returns>Time slot with given start time</returns>
        public TimeSlot GetTimeSlotByStart(TimeOnly start)
        {
            return timeSlots.First(t => t.Start.Equals(start));
        }

        public IEnumerable<TimeSlot> ToTimeSlots()
        {
            return timeSlots;
        }
    }


}