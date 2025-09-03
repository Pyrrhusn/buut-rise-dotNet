using Rise.Domain.TimeSlots;

namespace Rise.Domain.Tests.TestUtilities
{
    public class TimeSlotBuilder
    {
        public static readonly DateOnly ValidDate = DateOnly.FromDateTime(CruisePeriodBuilder.ValidStart);
        public static readonly TimeOnly ValidStart = new(10, 0, 0);
        public static readonly TimeOnly ValidEnd = new(13, 0, 0);

        private DateOnly date = ValidDate;
        private TimeOnly start = ValidStart;
        private TimeOnly end = ValidEnd;

        private CruisePeriod cruisePeriod = new CruisePeriodBuilder().Build();

        public TimeSlotBuilder WithDate(DateOnly newDate)
        {
            date = newDate;
            return this;
        }
        public TimeSlotBuilder WithDate(int days)
        {
            return WithDate(date.AddDays(days));
        }

        public TimeSlotBuilder WithStart(TimeOnly newStart)
        {
            start = newStart;
            return this;
        }

        public TimeSlotBuilder WithEnd(TimeOnly newEnd)
        {
            end = newEnd;
            return this;
        }

        public TimeSlotBuilder WithCruisePeriod(CruisePeriod newCruisePeriod)
        {
            cruisePeriod = newCruisePeriod;
            return this;
        }

        public TimeSlot Build()
        {
            return new TimeSlot
            {
                CruisePeriod = cruisePeriod,
                Date = date,
                Start = start,
                End = end,
            };
        }
    }


}