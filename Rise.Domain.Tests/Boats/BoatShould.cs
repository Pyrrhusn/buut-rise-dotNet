using Rise.Domain.Boats;
using Rise.Domain.Reservations;
using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.TimeSlots;
using Shouldly;

namespace Rise.Domain.Tests.Boats
{
    public class BoatShould
    {
        public const string ValidPersonalNameFormatted = "Limba";

        [Theory]
        [InlineData("")]
        [InlineData("\n")]
        public void BeCreated(string extras)
        {
            Boat b = new BoatBuilder()
                .WithPersonalName(BoatBuilder.ValidPersonalName + extras)
                .Build();

            b.PersonalName.ShouldBe(ValidPersonalNameFormatted);
            b.Reservations.ShouldBeEmpty();
            b.Batteries.ShouldBeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeCreatedWithInvalidPersonalName(string? personalName)
        {
            Action act = () =>
            {
                Boat boat = new BoatBuilder()
                .WithPersonalName(personalName!)
                .Build();
            };
            act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("PersonalName");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("   ")]
        [InlineData("")]
        public void NotBeChangedWithInvalidPersonalName(string? personalName)
        {
            Action act = () =>
            {
                Boat boat = new BoatBuilder()
                .Build();
                boat.PersonalName = personalName!;
            };
            act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("PersonalName");
        }

        [Fact]
        public void BeAbleToAddValidReservation()
        {
            Boat b = new BoatBuilder().Build();
            IReadOnlyList<Reservation> reservations = b.Reservations;
            Reservation reservation = new ReservationBuilder().Build();

            reservations.ShouldBeEmpty();
            b.AddReservation(reservation);

            reservations.Count.ShouldBe(1);
            reservations.ShouldContain(reservation);

        }

        [Fact]
        public void BeAbleToAddValidBattery()
        {
            Boat b = new BoatBuilder().Build();
            IReadOnlyList<Battery> batteries = b.Batteries;
            Battery battery = new BatteryBuilder().Build();

            batteries.ShouldBeEmpty();
            b.AddBattery(battery);

            batteries.Count.ShouldBe(1);
            batteries.ShouldContain(battery);
        }

        [Fact]
        public void BeAbleToAddReservationWithValidReservation()
        {
            Boat boat = new BoatBuilder().Build();
            boat.Reservations.ShouldBeEmpty();

            Reservation reservation = new ReservationBuilder().Build();
            boat.AddReservation(reservation);

            boat.Reservations.ShouldNotBeEmpty();
            boat.Reservations.ShouldContain(reservation);
        }

        [Fact]
        public void NotBeAbleToAddReservationWithInvalidReservation()
        {
            Boat boat = new BoatBuilder().Build();
            boat.Reservations.ShouldBeEmpty();

            Action act = () =>
            {
                boat.AddReservation(null!);
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("reservation");
        }

        [Fact]
        public void DoesNotFindAvailableReservationWhenNoBatteries()
        {
            Boat boat = new BoatBuilder().Build();

            TimeSlot timeSlot = new TimeSlotBuilder().Build();

            boat.FindAvailableBattery(timeSlot, DateTime.Now).ShouldBeNull();
        }

        [Fact]
        public void ThrowExceptionAvailableReservationWhenNoTimeSlot()
        {
            Boat boat = new BoatBuilder().Build();

            Action act = () =>
            {
                boat.FindAvailableBattery(null!, DateTime.Now);
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("timeSlot");
        }

        // TODO tests FindAvailableBattery

        // TODO tests AssignBatteriesToReservations
    }
}