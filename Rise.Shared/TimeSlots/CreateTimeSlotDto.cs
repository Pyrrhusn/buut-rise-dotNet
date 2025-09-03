using FluentValidation;

namespace Rise.Shared.TimeSlots;



public record class CreateTimeSlotDto
{
    public List<TimeSlotRange> TimeSlots { get; init; } = new();
    public int CruisePeriodId { get; init; }

    public class Validator : AbstractValidator<CreateTimeSlotDto>
    {
        private const double RequiredHourDuration = 1.5;
        public Validator()
        {
            RuleFor(x => x.TimeSlots)
                .NotEmpty().WithMessage("At least one time slot is required")
                .ForEach(slot =>
                {
                    slot.ChildRules(timeSlot =>
                    {
                        timeSlot.RuleFor(x => x.Start)
                            .NotEmpty().WithMessage("Start time is required");

                        timeSlot.RuleFor(x => x.End)
                            .NotEmpty().WithMessage("End time is required")
                            .GreaterThan(x => x.Start).WithMessage("End time must be after start time")
                            .Must((range, end) => IsValidDuration(range.Start, end))
                            .WithMessage($"Time slot must be at least {RequiredHourDuration} hours long");
                    });
                });

            RuleFor(x => x.TimeSlots)
                .Must(HasNoOverlaps)
                .WithMessage("Time slot overlaps with another time slot");

            RuleFor(x => x.CruisePeriodId)
                .NotEmpty().WithMessage("Cruise Period ID is required")
                .GreaterThan(0).WithMessage("Cruise Period ID must be a positive number");
        }

        private bool IsValidDuration(TimeOnly start, TimeOnly end)
        {
            var duration = end - start;
            return duration.TotalHours >= RequiredHourDuration;
        }

        private bool HasNoOverlaps(List<TimeSlotRange> timeSlots)
        {
            var sortedSlots = timeSlots.OrderBy(x => x.Start).ToList();

            for (int i = 0; i < sortedSlots.Count - 1; i++)
            {
                if (sortedSlots[i].End > sortedSlots[i + 1].Start)
                {
                    return false;
                }
            }
            return true;
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
            async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<CreateTimeSlotDto>
                    .CreateWithOptions((CreateTimeSlotDto)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return [];
                return result.Errors.Select(e => e.ErrorMessage);
            };



    }
}
public record class TimeSlotRange
{
    public TimeOnly Start { get; init; }
    public TimeOnly End { get; init; }
}

