using Rise.Domain.TimeSlots;
using Rise.Persistence.Seeders.CruisePeriods;

namespace Rise.Persistence.Seeders.Reservations
{
    internal class ReservationDay(ReservationPeriodSchedule periodSchedule, CruiseDay daySchedule)
    {
        private readonly CruiseDay daySchedule = daySchedule;

        public ReservationTimeSlot WithTimeSlot(int index)
        {
            return WithTimeSlot(daySchedule.ToTimeSlots().ToList()[index]);
        }

        public ReservationTimeSlot WithTimeSlot(TimeOnly start)
        {
            return WithTimeSlot(daySchedule.GetTimeSlotByStart(start));
        }

        public ReservationTimeSlot WithTimeSlot(TimeSlot timeSlot)
        {

            ReservationTimeSlot reservationTimeSlot = new(this, timeSlot);
            return reservationTimeSlot;
        }
        public ReservationPeriodSchedule Done()
        {
            return periodSchedule;
        }
    }
}