using System;
using FluentValidation;

namespace Rise.Shared.Boats;

public record class CreateBoatDto
{
    public required string PersonalName { get; set; }
    public required bool IsAvailable { get; set; }

    public class Validator : AbstractValidator<CreateBoatDto>
    {
        public Validator()
        {
            int personalNameMaxLength = 64;
            RuleFor(x => x.PersonalName)
            .NotEmpty().WithMessage("Personal name is required")
            .MaximumLength(personalNameMaxLength).WithMessage($"Personal name can't be longer than {personalNameMaxLength} characters.");

            RuleFor(x => x.IsAvailable)
            .NotNull().WithMessage("Availability is required");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<CreateBoatDto>.CreateWithOptions((CreateBoatDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}
