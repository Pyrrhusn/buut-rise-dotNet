using FluentValidation;

namespace Rise.Shared.Users;

public record class UserRegistrationModelDto
{
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string FamilyName { get; set; }
    public required string PhoneNumber { get; set; }
    public required DateTime? DateOfBirth { get; set; }
    public required AddressModel Address { get; set; }

    public record class AddressModel
    {
        public required string Street { get; set; }
        public required string Number { get; set; }
        public required string City { get; set; }
        public required string PostalCode { get; set; }
        public required string Country { get; set; }
    }

    public class Validator : AbstractValidator<UserRegistrationModelDto>
    {
        public const int emailMaxLength = 69;
        public const int passwordMaxLength = 64;
        public const int firstNameMaxLength = 100;
        public const int lastNameMaxLength = 100;
        public const int phoneNumberMaxLength = 35;
        public const int streetMaxLength = 200;
        public const int numberMaxLength = 25;
        public const int cityMaxLength = 200;
        public const int postalCodeMaxLength = 100;
        public const int countryMaxLength = 100;

        public Validator()
        {

            RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Please provide your email address")
            .MaximumLength(emailMaxLength).WithMessage($"Email can't be longer than {emailMaxLength} characters")
            .EmailAddress().WithMessage("The email address provided is not valid");


            RuleFor(x => x.Password).NotEmpty()
            .WithMessage("Please provide a password.")
            .MaximumLength(passwordMaxLength).WithMessage($"Password can't be longer than {passwordMaxLength} characters.")
            .Matches(".*[!@#$%^&*].*")
            .WithMessage("Password requires at least one special character: !@#$%^&*")
            .Matches(".*[a-z].*")
            .WithMessage("Password requires at least one lower case letter")
            .Matches(".*[A-Z].*")
            .WithMessage("Password requires at least one upper case letter")
            .Matches(".*[0-9].*")
            .WithMessage("Password requires at least one number")
            .Matches(".{8,}")
            .WithMessage("Password requires at least 8 characters");


            RuleFor(x => x.FirstName).NotEmpty()
            .WithMessage("Please provide your first name")
            .MaximumLength(firstNameMaxLength).WithMessage($"First name can't be longer than {firstNameMaxLength} characters");


            RuleFor(x => x.FamilyName).NotEmpty()
            .WithMessage("Please provide your last name")
            .MaximumLength(lastNameMaxLength).WithMessage($"Last name can't be longer than {lastNameMaxLength} characters");


            RuleFor(x => x.PhoneNumber).NotEmpty()
            .WithMessage("Please provide your phone number")
            .MaximumLength(phoneNumberMaxLength).WithMessage($"Phone number name can't be longer than {phoneNumberMaxLength} characters")
            .Matches("^[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\\s\\./0-9]*$")
            .WithMessage("The phone number provided is not valid");

            RuleFor(x => x.DateOfBirth)
            .NotNull().WithMessage("Please provide your date of birth.")
            .NotEmpty().WithMessage("Please provide your date of birth.")
            .Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old to register.");


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

        private bool BeAtLeast18YearsOld(DateTime? dateOfBirth)
        {
            return dateOfBirth.HasValue && dateOfBirth.Value <= DateTime.Today.AddYears(-18);
        }

        public Func<object, string, Task<IEnumerable<string>>>  ValidateValue => async (model, propertyName) =>
        {
            var result = await ValidateAsync(ValidationContext<UserRegistrationModelDto>.CreateWithOptions((UserRegistrationModelDto)model, x => x.IncludeProperties(propertyName)));
            if (result.IsValid)
                return Array.Empty<string>();
            return result.Errors.Select(e => e.ErrorMessage);
        };
    }
}