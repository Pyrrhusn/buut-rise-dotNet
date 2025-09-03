namespace Rise.Shared.Reservations;
using System.ComponentModel.DataAnnotations;
using FluentValidation;

public record class CreateReservationDto
{
    /// <summary>
    /// The ID of the time slot to reserve.
    /// </summary>
    public int TimeSlotId { get; set; }


    public class Validator : AbstractValidator<CreateReservationDto>
    {
        public Validator()
        {
            RuleFor(x => x.TimeSlotId)
            .NotEmpty().WithMessage("Time Slot ID is required")
            .GreaterThan(0).WithMessage("Time Slot ID must be a positive number");

        }
    }

}
