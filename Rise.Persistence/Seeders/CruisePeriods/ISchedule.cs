using Rise.Domain.TimeSlots;

namespace Rise.Persistence.Seeders.CruisePeriods
{
    interface ITimedSchedule
    {
        /// <summary>
        /// Flattens the schedule into a list of time slots
        /// </summary>
        /// <returns>Time slots</returns>
        public IEnumerable<TimeSlot> ToTimeSlots();
    }

}