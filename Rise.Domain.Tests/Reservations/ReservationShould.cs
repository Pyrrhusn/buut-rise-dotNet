using Shouldly;
using Rise.Domain.Reservations;
using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.Boats;

namespace Rise.Domain.Tests.Reservations
{
    public class ReservationShould
    {
        [Fact]
        public void BeCreatedWithUser()
        {
            Reservation reservation = new ReservationBuilder().Build();


            reservation.Boat.ShouldBe(ReservationBuilder.ValidBoat);
            reservation.TimeSlot.ShouldBe(ReservationBuilder.ValidTimeSlot);
            reservation.User.ShouldBe(ReservationBuilder.ValidUser);
        }

        [Fact]
        public void NotBeCreatedIfBoatIsAlreadyReservedForTimeSlot()
        {

            Reservation reservation1 = new ReservationBuilder().Build();

            Reservation reservation2 = new ReservationBuilder()
            .WithUser(
                new UserBuilder()
                    .WithFamilyName("Other user")
                    .Build()
            )
            .Build();

            CheckBoatAvailability(reservation1, reservation2).ShouldBeFalse("Boat should not be available for the same time slot.");
        }

        [Fact]
        public void ShouldEnforceMinimumDaysBetweenReservations()
        {
            Reservation reservation1 = new ReservationBuilder().Build();

            Reservation reservation2 = new ReservationBuilder()
            .WithTimeSlot(
                new TimeSlotBuilder()
                .WithDate(DateOnly.FromDateTime(DateTime.Now.AddDays(1)))
                .Build()
            )
            .Build();

            IsValidReservationDate(reservation1, reservation2).ShouldBeFalse(
                         "Reservation should not be allowed within minimum 2 days.");
        }

        private static bool CheckBoatAvailability(Reservation reservation1, Reservation reservation2)
        {
            return !(reservation1.BoatId == reservation2.BoatId &&
                     reservation1.TimeSlotId == reservation2.TimeSlotId);
        }

        private static bool IsValidReservationDate(Reservation reservation1, Reservation reservation2)
        {
            return (reservation2.TimeSlot.Date.DayNumber - reservation1.TimeSlot.Date.DayNumber)
                   >= Reservation.MinDaysBetweenReservation;
        }

        [Fact]
        public void CancelReservationSuccessfully_AsAdmin_WhenWithinTwoDays()
        {
            
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1))) 
                        .Build()
                )
                .Build();

            
            reservation.Cancel(isAdmin: true);

            
            reservation.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public void CancelReservationSuccessfully_AsAdmin_OnSameDay()
        {
            

            var startDate = DateTime.Today;
            var endDate = startDate.AddDays(5);
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                    .WithCruisePeriod(new CruisePeriodBuilder()
                    .WithStart(startDate)
                    .WithEnd(endDate)
                    .Build())
                        .WithDate(DateOnly.FromDateTime(DateTime.Today))
                        .Build()
                )
                .Build();

            
            reservation.Cancel(isAdmin: true);

            
            reservation.IsDeleted.ShouldBeTrue();
        }

        [Fact]
        public void ThrowException_WhenAdminCancelsPastReservation()
        {
            
            var startDate = DateTime.Today.AddDays(-(1));
            var endDate = startDate.AddDays(5);
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                    .WithCruisePeriod(new CruisePeriodBuilder()
                    .WithStart(startDate)
                    .WithEnd(endDate)
                    .Build())
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
                        .Build()
                )
                .Build();

            
            Should.Throw<InvalidOperationException>(() => reservation.Cancel(isAdmin: true))
                .Message.ShouldBe("Reservations in the past cannot be canceled.");
        }

        [Fact]
        public void ThrowException_WhenUserCancelsPastReservation()
        {
            
            var startDate = DateTime.Today.AddDays(-(1));
            var endDate = startDate.AddDays(5);
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                    .WithCruisePeriod(new CruisePeriodBuilder()
                    .WithStart(startDate)
                    .WithEnd(endDate)
                    .Build())
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(-1)))
                        .Build()
                )
                .Build();

            
            Should.Throw<InvalidOperationException>(() => reservation.Cancel(isAdmin: false))
                .Message.ShouldBe("Reservations in the past cannot be canceled.");
        }

        [Fact]
        public void ThrowException_WhenAdminTriesToCancelAlreadyCancelledReservation()
        {
            
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(5)))
                        .Build()
                )
                .Build();

            reservation.Cancel(isAdmin: true); 

            
            Should.Throw<InvalidOperationException>(() => reservation.Cancel(isAdmin: true))
                .Message.ShouldBe("The reservation is already canceled.");
        }

        [Fact]
        public void NotThrowException_WhenAdminCancelsExactlyTwoDaysBefore()
        {
            
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2))) 
                        .Build()
                )
                .Build();

            
            reservation.Cancel(isAdmin: true);

            
            reservation.IsDeleted.ShouldBeTrue();
            reservation.Battery.ShouldBe(null);
        }

        [Fact]
        public void EnforceTwoDayRule_WhenUserCancelsReservation()
        {
            
            var reservation = new ReservationBuilder()
                .WithTimeSlot(
                    new TimeSlotBuilder()
                        .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(1))) 
                        .Build()
                )
                .Build();

            
            Should.Throw<InvalidOperationException>(() => reservation.Cancel(isAdmin: false))
                .Message.ShouldBe("Reservations can only be canceled at least 2 days before the reservation date unless canceled by an admin.");
        }

        // TODO tests for cancel with battery
        // [Fact]
        // public void NotThrowException_WhenCancellationMomentIsValidAndHasBatteryAssigned()
        // {
        //     var reservation = new ReservationBuilder()
        //         .WithTimeSlot(
        //             new TimeSlotBuilder()
        //                 .WithDate(DateOnly.FromDateTime(DateTime.Today.AddDays(2))) // Exactly 2 days
        //                 .Build()
        //         )
        //         .WithBattery()
        //         .Build();

        //     reservation.Cancel();

        //     reservation.IsDeleted.ShouldBeTrue();
        //     reservation.Battery.ShouldBe(null);
        // }

        // TODO tests for AssignBattery


    }
}
