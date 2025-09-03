using FluentValidation;
using Rise.Shared.Address;

namespace Rise.Shared.Users;

public class UpdateUserProfileDto
{
    public required string FirstName { get; set; }
    public required string FamilyName { get; set; }
    public required string PhoneNumber { get; set; }
    public required AddressDto Address { get; set; }

    public class Validator : AbstractValidator<UpdateUserProfileDto>
    {
        public const int firstNameMaxLength = 100;
        public const int familyNameMaxLength = 100;
        public const int phoneNumberMaxLength = 35;
        public const int streetMaxLength = 200;
        public const int numberMaxLength = 25;
        public const int cityMaxLength = 200;
        public const int postalCodeMaxLength = 100;
        public const int countryMaxLength = 100;

        public Validator()
        {
            RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("Please provide your first name")
            .MaximumLength(firstNameMaxLength).WithMessage($"First name can't be longer than {firstNameMaxLength} characters");


            RuleFor(x => x.FamilyName).NotEmpty()
            .WithMessage("Please provide your family name")
            .MaximumLength(familyNameMaxLength).WithMessage($"Last name can't be longer than {familyNameMaxLength} characters");


            RuleFor(x => x.PhoneNumber).NotEmpty()
            .WithMessage("Please provide your phone number")
            .MaximumLength(phoneNumberMaxLength).WithMessage($"Phone number name can't be longer than {phoneNumberMaxLength} characters")
            .Matches("^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\\s\\./0-9]*$")
            .WithMessage("The phone number provided is not valid");


            RuleFor(x => x.Address.Street).NotEmpty()
            .WithMessage("Please provide your street address")
            .MaximumLength(streetMaxLength).WithMessage($"Street name can't be longer than {streetMaxLength} characters");


            RuleFor(x => x.Address.Number).NotEmpty()
            .WithMessage("Please provide your house number")
            .MaximumLength(numberMaxLength).WithMessage($"Your house number can't be longer than {numberMaxLength} characters");


            RuleFor(x => x.Address.City).NotEmpty()
            .WithMessage("Please provide your city")
            .MaximumLength(cityMaxLength).WithMessage($"City name can't be longer than {cityMaxLength} characters");


            RuleFor(x => x.Address.PostalCode).NotEmpty()
            .WithMessage("Please provide your postal code")
            .MaximumLength(postalCodeMaxLength).WithMessage($"Postal code can't be longer than {postalCodeMaxLength} characters");


            RuleFor(x => x.Address.Country).NotEmpty()
            .WithMessage("Please provide your country")
            .MaximumLength(countryMaxLength).WithMessage($"Country name can't be longer than {countryMaxLength} characters");
        }

        public Func<object, string, Task<IEnumerable<string>>> ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UpdateUserProfileDto>.CreateWithOptions((UpdateUserProfileDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return [];
            return result.Errors.Select(e => e.ErrorMessage);
        };

    }
}
