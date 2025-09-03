using Rise.Domain.Reservations;

namespace Rise.Domain.TimeSlots;

public class TimeSlot : Entity
{
    private TimeOnly _start;
    private TimeOnly _end;
    private DateOnly _date;

    public required DateOnly Date
    {
        get => _date;
        set
        {
            Guard.Against.OutOfRange(value.Year, nameof(Date), 2000, DateTime.Now.Year + 100,
                "TimeSlot date must be after year 2000.");

            DateOnly startDate = DateOnly.FromDateTime(CruisePeriod.Start);
            DateOnly endDate = DateOnly.FromDateTime(CruisePeriod.End);

            Guard.Against.OutOfRange(value, nameof(Date), startDate, endDate,
                "TimeSlot date must be within the CruisePeriod's date range.");

            _date = value;
        }
    }

    public required TimeOnly Start
    {
        get => _start;
        set
        {
            Guard.Against.OutOfRange(value, nameof(Start), TimeOnly.MinValue, TimeOnly.MaxValue, "Start time must be within a valid range.");

            _start = value;
        }
    }

    public required TimeOnly End
    {
        get => _end;
        set
        {
            Guard.Against.OutOfRange(value, nameof(End), TimeOnly.MinValue, TimeOnly.MaxValue,
                "End time must be within a valid range.");
            Guard.Against.OutOfRange(value, nameof(End), Start.AddMinutes(1), TimeOnly.MaxValue,
                "End time must be after Start time.");
            _end = value;
        }
    }

    public required CruisePeriod CruisePeriod { get; set; }

    private readonly List<Reservation> reservations = [];
    public IReadOnlyList<Reservation> Reservations => reservations.AsReadOnly();

    // TODO test method
    public DateTime StartDateTime => Date.ToDateTime(Start);
    public DateTime EndDateTime => Date.ToDateTime(End);
}