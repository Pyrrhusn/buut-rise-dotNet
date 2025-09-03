

using Rise.Domain.Exceptions;
namespace Rise.Domain.TimeSlots;
public class CruisePeriod : Entity
{

    private DateTime _start;
    private DateTime _end;

    public DateTime Start
    {
        get => _start;
        set
        {
            // TODO check against null
            Guard.Against.OutOfSQLDateRange(value, nameof(Start));
            _start = value;
        }
    }

    public DateTime End
    {
        get => _end;
        set
        {
            // TODO check against null
            Guard.Against.OutOfSQLDateRange(value, nameof(End));
            Guard.Against.OutOfRange(value, nameof(End), _start, DateTime.MaxValue, "End date must be after Start date.");
            _end = value;
        }
    }
    private readonly List<TimeSlot> timeSlots = [];

    public IReadOnlyList<TimeSlot> TimeSlots => timeSlots.AsReadOnly();

    /// <summary>
    /// Adds a time slot to the given cruise period.
    /// </summary>
    /// <param name="timeSlot">The time slot to add. This should be a valid time within the allowed range of the cruise period.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when the time slot's date and start time are out of the valid range specified by the cruise's start and end time.
    /// </exception>
    public void AddTimeSlot(TimeSlot timeSlot)
    {
        bool duplicateExists = timeSlots.Any(ts =>
            !ts.IsDeleted &&
            ts.Date == timeSlot.Date &&
            ts.Start == timeSlot.Start &&
            ts.End == timeSlot.End &&
            ts.CruisePeriod.Id == this.Id);

        if (duplicateExists)
        {
            throw new EntityAlreadyExistsException(
                nameof(TimeSlot),
                "DateTime",
                $"Date: {timeSlot.Date}, Time: {timeSlot.Start}-{timeSlot.End}"
            );
        }

        // TODO check against null
        Guard.Against.OutOfRange(
            timeSlot.Date.ToDateTime(timeSlot.Start),
            nameof(AddTimeSlot),
            Start,
            End
        );
        timeSlots.Add(timeSlot);
    }


    /// <summary>
    /// Adds time slots for a range of dates.
    /// </summary>
    /// <param name="start">start timeslot</param>
    /// <param name="end">end timeslot</param>
    public void AddTimeSlots(TimeOnly startTime, TimeOnly endTime)
    {
        var currentDate = DateOnly.FromDateTime(Start);
        //subtract one day from the end date 
        //cruiseperiodes are always date: 00:00:00, so in the end date we want to include the last day
        var endDate = DateOnly.FromDateTime(End.Add(TimeSpan.FromDays(-1)));

        while (currentDate <= endDate)
        {
            var timeSlot = new TimeSlot
            {
                CruisePeriod = this,
                Date = currentDate,
                Start = startTime,
                End = endTime,
            };

            AddTimeSlot(timeSlot);
            currentDate = currentDate.AddDays(1);
        }
    }


}
