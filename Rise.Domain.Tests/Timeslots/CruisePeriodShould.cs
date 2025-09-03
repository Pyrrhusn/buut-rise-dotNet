using Rise.Domain.TimeSlots;
using Rise.Domain.Tests.TestUtilities;
using Shouldly;
using Rise.Domain.Exceptions;

namespace Rise.Domain.Tests.TimeSlots
{
    public class CruisePeriodShould
    {

        [Fact]
        public void BeCreated()
        {
            CruisePeriod cruisePeriod = new CruisePeriodBuilder().Build();

            cruisePeriod.Start.ShouldBe(CruisePeriodBuilder.ValidStart);
            cruisePeriod.End.ShouldBe(CruisePeriodBuilder.ValidEnd);
            cruisePeriod.TimeSlots.ShouldNotBeNull();
        }

        [Fact]
        public void NotBeCreatedWithEndBeforeStart()
        {
            Action act = () =>
            {
                CruisePeriod cruisePeriod = new CruisePeriodBuilder().WithStart(CruisePeriodBuilder.ValidEnd).WithEnd(CruisePeriodBuilder.ValidStart).Build();
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeChangedToHaveAnInvalidEnd(string endString)
        {
            DateTime invalidEnd = DateTime.Parse(endString);

            Action act = () =>
            {
                CruisePeriod cruisePeriod = new CruisePeriodBuilder().Build();
                cruisePeriod.End = invalidEnd;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void NotBeChangedToHaveEndBeforeStart()
        {
            Action act = () =>
            {
                CruisePeriod cruisePeriod = new CruisePeriodBuilder().Build();
                cruisePeriod.End = CruisePeriodBuilder.ValidStart.AddDays(-1);
            };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public void BeAbleToAddValidTimeSlot(int amountDays)
        {
            CruisePeriod period = new CruisePeriodBuilder().Build();
            IReadOnlyList<TimeSlot> timeSlots = period.TimeSlots;
            TimeSlot timeSlot = new TimeSlotBuilder()
            .WithDate(amountDays)
            .Build();

            timeSlots.ShouldBeEmpty();
            period.AddTimeSlot(timeSlot);

            timeSlots.Count.ShouldBe(1);
            timeSlots.ShouldContain(timeSlot);
        }

        [Theory]
        [InlineData(-2)]
        [InlineData(-1)]
        [InlineData(25)]
        [InlineData(26)]
        public void NotBeAbleToAddInvalidTimeSlot(int amountDays)
        {
            CruisePeriod period = new CruisePeriodBuilder().Build();
            IReadOnlyList<TimeSlot> timeSlots = period.TimeSlots;
            Action act = () =>
                    {
                        timeSlots.ShouldBeEmpty();
                        TimeSlot timeSlot = new TimeSlotBuilder()
                        .WithCruisePeriod(period)
                        .WithDate(amountDays)
                        .Build();
                    };

            act.ShouldThrow<ArgumentOutOfRangeException>();
        }

        [Fact]
        public void AddTimeSlots_ShouldCreateTimeSlotForEachDay()
        {
            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(17, 0);
            var period = new CruisePeriodBuilder().Build();

            period.AddTimeSlots(startTime, endTime);

            var firstSlot = period.TimeSlots.First();
            firstSlot.Start.ShouldBe(startTime);
            firstSlot.End.ShouldBe(endTime);
            firstSlot.Date.ShouldBe(DateOnly.FromDateTime(CruisePeriodBuilder.ValidStart));


            var lastSlot = period.TimeSlots.Last();
            lastSlot.Start.ShouldBe(startTime);
            lastSlot.End.ShouldBe(endTime);
            lastSlot.Date.ShouldBe(DateOnly.FromDateTime(CruisePeriodBuilder.ValidEnd.AddDays(-1)));
        }

        [Fact]
        public void AddTimeSlots_ShouldThrowWhenAddingDuplicateTimeSlots()
        {
            var startTime = new TimeOnly(9, 0);
            var endTime = new TimeOnly(17, 0);
            var period = new CruisePeriodBuilder().Build();

            period.AddTimeSlots(startTime, endTime);

            var act = () => period.AddTimeSlots(startTime, endTime);

            act.ShouldThrow<EntityAlreadyExistsException>()
               .Message.ShouldContain(DateOnly.FromDateTime(CruisePeriodBuilder.ValidStart).ToString());
        }

        [Theory]
        [InlineData(23, 0, 23, 59)]
        [InlineData(0, 0, 23, 0)]
        [InlineData(12, 0, 13, 50)]
        public void AddTimeSlots_ShouldHandleVariousTimeRanges(
            int startHour, int startMinute,
            int endHour, int endMinute)
        {
            var startTime = new TimeOnly(startHour, startMinute);
            var endTime = new TimeOnly(endHour, endMinute);
            var period = new CruisePeriodBuilder().Build();

            period.AddTimeSlots(startTime, endTime);

            period.TimeSlots.Count.ShouldBe(4);
            period.TimeSlots.ShouldAllBe(ts =>
                ts.Start == startTime &&
                ts.End == endTime);
        }


    }


}