using Rise.Domain.Boats;
using Rise.Domain.TimeSlots;
using Rise.Domain.Users;

namespace Rise.Persistence.Seeders.Reservations
{
    internal class ReservationTimeSlot(ReservationDay reservationDay, TimeSlot timeSlot)
    {
        public ReservationTimeSlot AddReservation(Boat boat, User user)
        {
            boat.AddReservation(
                new()
                {
                    Boat = boat,
                    TimeSlot = timeSlot,
                    User = user
                }
            );
            return this;
        }
        public ReservationDay Done()
        {
            return reservationDay;
        }
    }
}