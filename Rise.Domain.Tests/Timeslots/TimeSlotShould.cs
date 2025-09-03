using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.TimeSlots;
using Shouldly;

namespace Rise.Domain.Tests.TimeSlots
{
    public class TimeSlotShould
    {

        [Fact]
        public void BeCreated()
        {
            TimeSlot timeSlot = new TimeSlotBuilder().Build();

            timeSlot.Date.ShouldBe(TimeSlotBuilder.ValidDate);
            timeSlot.Start.ShouldBe(TimeSlotBuilder.ValidStart);
            timeSlot.End.ShouldBe(TimeSlotBuilder.ValidEnd);
            timeSlot.Reservations.ShouldBeEmpty();
        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeCreatedWithAnInvalidDate(string dateString)
        {
            DateOnly invalidDate = DateOnly.Parse(dateString);

            Action act = () =>
            {
                TimeSlot timeSlot = new TimeSlotBuilder().WithDate(invalidDate).Build();
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
            .ParamName.ShouldBe("Date");
        }

        [Fact]
        public void NotBeCreatedWithEndBeforeStart()
        {
            Action act = () =>
            {
                TimeSlot timeSlot = new TimeSlotBuilder()
                .WithStart(TimeSlotBuilder.ValidEnd)
                .WithEnd(TimeSlotBuilder.ValidStart)
                .Build();
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("End");

        }

        [Theory]
        [InlineData("0001-01-01")]
        public void NotBeChangedToHaveAnInvalidDate(string dateString)
        {
            DateOnly invalidDate = DateOnly.Parse(dateString);
            TimeSlot timeSlot = new TimeSlotBuilder().Build();

            Action act = () =>
            {
                timeSlot.Date = invalidDate;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
            .ParamName.ShouldBe("Date");
        }

        [Fact]
        public void NotBeChangedToHaveEndBeforeStart()
        {
            TimeSlot timeSlot = new TimeSlotBuilder().Build();
            TimeOnly invalidEnd = TimeSlotBuilder.ValidStart;

            Action act = () =>
            {
                timeSlot.End = invalidEnd;
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("End");

        }

        [Fact]
        public void NotBeCreatedWithDateOutsideCruisePeriod()
        {
            // Arrange
            DateTime endDate = DateTime.Today.AddDays(25);

            // Act
            Action act = () =>
            {
                TimeSlot timeSlot = new TimeSlotBuilder()
                .WithCruisePeriod(
                    new CruisePeriodBuilder()
                        .WithEnd(endDate)
                        .Build()
                )
                .WithDate(DateOnly.FromDateTime(endDate.AddDays(1)))
                .Build();
            };

            // Assert
            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("Date");
        }


        [Theory]
        [InlineData(1999, 1, 1)] // Year before 2000
        [InlineData(2125, 1, 1)] // Year after more than 100 years to-date.
        public void NotBeCreatedWithDateOutsideValidYearRange(int year, int month, int day)
        {
            Action act = () =>
            {
                TimeSlot timeSlot = new TimeSlotBuilder()
                .WithDate(new DateOnly(year, month, day))
                .Build();
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("Date");
        }

        [Fact]
        public void NotBeChangedToHaveDateOutsideValidYearRange()
        {
            TimeSlot timeSlot = new TimeSlotBuilder().Build();

            Action act = () =>
            {
                timeSlot.Date = new DateOnly(1999, 1, 1);
            };

            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("Date");
        }

        [Fact]
        public void NotBeChangedToHaveDateOutsideCruisePeriod()
        {
            // Arrange
            DateTime endDate = DateTime.Today.AddDays(25);
            TimeSlot timeSlot = new TimeSlotBuilder()
                .WithCruisePeriod(
                    new CruisePeriodBuilder()
                        .WithEnd(endDate)
                        .Build()
                )
                .Build();

            // Act
            Action act = () =>
            {
                timeSlot.Date = DateOnly.FromDateTime(endDate.AddDays(1));
            };

            // Assert
            act.ShouldThrow<ArgumentOutOfRangeException>()
                .ParamName.ShouldBe("Date");
        }

        [Fact]
        public void AllowDateChangeWithinCruisePeriod()
        {

            // Arrange
            DateTime endDate = DateTime.Today.AddDays(5);
            TimeSlot timeSlot = new TimeSlotBuilder()
                .WithCruisePeriod(
                    new CruisePeriodBuilder()
                        .WithEnd(endDate)
                        .Build()
                )
                .Build();

            DateOnly newDate = DateOnly.FromDateTime(DateTime.Today.AddDays(3));
            timeSlot.Date = newDate;

            timeSlot.Date.ShouldBe(newDate);
        }

        [Fact]
        public void CorrectlyMakeStartDateTime()
        {
            TimeSlot timeSlot = new TimeSlotBuilder().Build();

            timeSlot.StartDateTime.ShouldBe(timeSlot.Date.ToDateTime(timeSlot.Start));
        }

        [Fact]
        public void CorrectlyMakeEndDateTime()
        {
            TimeSlot timeSlot = new TimeSlotBuilder().Build();

            timeSlot.EndDateTime.ShouldBe(timeSlot.Date.ToDateTime(timeSlot.End));
        }

    }
}