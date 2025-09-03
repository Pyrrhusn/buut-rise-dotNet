using FluentValidation;

namespace Rise.Shared.Boats
{
    /// <summary>
    /// Info for a battery to update them
    /// </summary>
    public record BatteryUpdateDto
    {
        public required string Type { get; set; }
        public required int MentorId { get; set; }

        public class Validator : AbstractValidator<BatteryUpdateDto>
        {
            public Validator()
            {
                RuleFor(x => x.Type);

                RuleFor(x => x.MentorId)
                .NotNull()
                .GreaterThan(0)
                ;
            }

            public Func<object, string, Task<IEnumerable<string>>> ValidateValue =>
            async (model, propertyName) =>
            {
                var result = await ValidateAsync(ValidationContext<BatteryUpdateDto>.CreateWithOptions((BatteryUpdateDto)model, x => x.IncludeProperties(propertyName)));
                if (result.IsValid)
                    return [];
                return result.Errors.Select(e => e.ErrorMessage);
            };
        }
    }
}


