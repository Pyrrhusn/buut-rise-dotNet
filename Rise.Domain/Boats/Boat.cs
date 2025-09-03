using Rise.Domain.Reservations;
using Rise.Domain.TimeSlots;

namespace Rise.Domain.Boats
{
    public class Boat : Entity
    {
        private string _personalName = default!;

        public required string PersonalName
        {
            get => _personalName;
            set
            {
                Guard.Against.NullOrWhiteSpace(value, nameof(PersonalName));
                Guard.Against.LengthOutOfRange(value, 1, 64, nameof(PersonalName));
                _personalName = value.Trim();
            }
        }

        /// <summary>
        /// Indicates whether the boat is available for reservations
        /// </summary>
        private bool _isAvailable = true;

        public bool IsAvailable
        {
            get => _isAvailable;
            set => _isAvailable = Guard.Against.Null(value, nameof(IsAvailable), "Availability status cannot be null");
        }

        private readonly List<Reservation> reservations = [];
        public IReadOnlyList<Reservation> Reservations => reservations.AsReadOnly();

        private readonly List<Battery> batteries = [];

        public IReadOnlyList<Battery> Batteries => batteries.AsReadOnly();

        public void AddReservation(Reservation reservation)
        {
            Guard.Against.Null(reservation, nameof(reservation));
            reservations.Add(reservation);
        }

        public void AddBattery(Battery battery)
        {
            Guard.Against.Null(battery, nameof(battery));
            batteries.Add(battery);
        }

        public Battery? FindAvailableBattery(TimeSlot timeSlot, DateTime currentTime)
        {
            Guard.Against.Null(timeSlot);
            Guard.Against.Null(currentTime);

            IEnumerable<Battery> compatibleBatteries = batteries
                .OrderBy(b => b.UsageCount)
                .ThenBy(b => b.Type);

            return compatibleBatteries.FirstOrDefault(
                b => b.IsAvailableForTimeSlot(timeSlot));
        }

        public List<Reservation> AssignBatteriesToReservations(DateTime now)
        {
            DateOnly today = DateOnly.FromDateTime(now);

            return reservations
                .Where(r =>
                        !r.IsDeleted &&
                        today <= r.TimeSlot.Date &&
                        r.TimeSlot.Date <= today.AddDays(Reservation.MinDaysBetweenReservation)
                    )
                .OrderBy(r => r.TimeSlot.Date)
                .ThenBy(r => r.TimeSlot.Start)
                .Where(r => r.Battery is null)
                .Select(reservation =>
                {
                    var battery = FindAvailableBattery(reservation.TimeSlot, now);
                    if (battery is not null)
                    {
                        return reservation.AssignBattery(battery);
                    }
                    return null;
                })
                .Where(r => r is not null)
                .ToList()!;
        }

        /// <summary>
        /// Updates the availability of the boat
        /// </summary>
        /// <param name="isAvailable">New availability status</param>
        public void ChangeAvailability(bool isAvailable)
        {
            IsAvailable = isAvailable;
        }
    }
}