using Rise.Domain.Reservations;
using Rise.Domain.TimeSlots;
using Rise.Domain.Users;

namespace Rise.Domain.Boats
{
    public class Battery : Entity
    {
        private const int rechargeBufferHours = 4;
        private string _type = default!;
        private readonly List<Reservation> _reservations = [];
        private int _usageCount;

        public int UsageCount => _usageCount;

        private User _mentor = default!;
        public required User Mentor
        {
            get => _mentor;
            set => _mentor = Guard.Against.Null(value, nameof(Mentor), "Mentor cannot be null or empty");
        }

        public required Boat Boat { get; set; }
        public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

        public string Type
        {
            get => _type;
            set => _type = Guard.Against.NullOrWhiteSpace(value, nameof(Type), "Battery type cannot be null or empty");
        }

        public void IncreaseUsageStats()
        {
            _usageCount++;
        }

        public void DecreaseUsageStats()
        {
            Guard.Against.NegativeOrZero(UsageCount);
            _usageCount--;
        }

        public void AddReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Add(reservation);
        }

        public void RemoveReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            _reservations.Remove(reservation);
        }

        public bool IsAvailableForTimeSlot(TimeSlot timeSlot)
        {
            int above = 24 - rechargeBufferHours;

            return !Reservations
                .Where(r => r.TimeSlot.Date == timeSlot.Date)
                .Any(r =>
                {
                    int hourDifference = (r.TimeSlot.End - timeSlot.Start).Hours;
                    return hourDifference < rechargeBufferHours || above < hourDifference;
                });
        }

        public Reservation? ClosesPastReservation(TimeSlot timeSlot)
        {
            Guard.Against.Null(timeSlot);

            DateTime startTimeSlot = timeSlot.StartDateTime;
            IEnumerable<Reservation> pastReservations = Reservations.Where(r => r.TimeSlot.Date <= timeSlot.Date);

            Reservation? closesReservation = pastReservations.OrderBy(
                r => startTimeSlot.Subtract(r.TimeSlot.StartDateTime).TotalMinutes
            ).FirstOrDefault();

            return closesReservation;
        }

        public Reservation? ClosesFutureReservation(TimeSlot timeSlot)
        {
            DateTime startTimeSlot = timeSlot.StartDateTime;
            IEnumerable<Reservation> futureReservations = Reservations.Where(r => r.TimeSlot.Date >= timeSlot.Date);

            Reservation? closesReservation = futureReservations.OrderBy(
                r => r.TimeSlot.StartDateTime.Subtract(startTimeSlot).TotalMinutes
            ).FirstOrDefault();

            return closesReservation;
        }
    }
}
