using Rise.Domain.Users;

namespace Rise.Domain.Tests.TestUtilities
{
    public class UserBuilder
    {
        public const string ValidEmail = "bob.her.der.gaver@gmail.com";
        public const string ValidFirstName = "Bob";
        public const string ValidFamilyName = "Her De Gaver";
        public const string ValidPhoneNumber = "04584475263";

        public static readonly DateTime ValidDateOfBirth = new(2005, 1, 1);

        public static readonly User.UserAddress ValidAddress = new()
        {
            City = "Gent",
            Country = "België",
            Number = "10",
            Street = "Fabiolalaan",
            PostalCode = "9000"
        };

        private string email = ValidEmail;
        private string firstName = ValidFirstName;
        private string familyName = ValidFamilyName;
        private string phoneNumber = ValidPhoneNumber;

        private DateTime dateOfBirth = ValidDateOfBirth;
        private User.UserAddress address = ValidAddress;

        public UserBuilder WithEmail(string email)
        {
            this.email = email;
            return this;
        }
        public UserBuilder WithFirstName(string firstName)
        {
            this.firstName = firstName;
            return this;
        }
        public UserBuilder WithFamilyName(string familyName)
        {
            this.familyName = familyName;
            return this;
        }
        public UserBuilder WithPhoneNumber(string phoneNumber)
        {
            this.phoneNumber = phoneNumber;
            return this;
        }

        public UserBuilder WithDateOfBirth(DateTime date)
        {
            this.dateOfBirth = date;
            return this;
        }

        public UserBuilder WithAddress(User.UserAddress address)
        {
            this.address = address;
            return this;
        }

        public User Build()
        {
            return new()
            {
                Email = email,
                FirstName = firstName,
                FamilyName = familyName,
                PhoneNumber = phoneNumber,
                DateOfBirth = dateOfBirth,
                Address = address,
            };
        }

    }
}
