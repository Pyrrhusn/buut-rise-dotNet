using Rise.Domain.TimeSlots;

namespace Rise.Domain.Tests.TestUtilities
{
    public class CruisePeriodBuilder
    {
        public static readonly DateTime ValidStart = DateTime.Today.AddDays(1);
        public static readonly DateTime ValidEnd = DateTime.Today.AddDays(5).AddHours(23);

        private DateTime start = ValidStart;
        private DateTime end = ValidEnd;

        public CruisePeriodBuilder WithStart(DateTime start)
        {
            this.start = start;
            return this;
        }

        public CruisePeriodBuilder WithEnd(DateTime end)
        {
            this.end = end;
            return this;
        }

        public CruisePeriod Build()
        {
            return new()
            {
                Start = start,
                End = end
            };
        }

    }

}

