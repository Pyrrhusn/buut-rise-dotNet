using Rise.Domain.Tests.TestUtilities;
using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Users
{
    public class UserShould
    {
        private const string ValidEmail = "test.user@domain.com";
        private const string ValidFirstName = "John";
        private const string ValidFamilyName = "Doe";
        private const string ValidPhoneNumber = "+1234567890";
        private static readonly DateTime ValidDateOfBirth = new(2005, 1, 1);
        private static readonly User.UserAddress ValidAddress = new()
        {
            Street = "123 Main St.",
            Number = "4B",
            City = "Springfield",
            PostalCode = "12345",
            Country = "USA"
        };

        #region Email Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-email")]
        public void NotBeCreatedWithInvalidEmail(string email)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithEmail(email).Build();
            }).ParamName.ShouldBe("Email");
        }

        [Fact]
        public void NotBeCreatedWithTooLongEmail()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithEmail("a." + new string('b', 63) + "@b.co").Build();
            }).ParamName.ShouldBe("maxLength");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-email")]
        public void NotBeChangedToHaveInvalidEmail(string email)
        {
            Action act = () =>
            {
                User user = new UserBuilder()
                    .Build();
                user.Email = email;
            };

            act.ShouldThrow<ArgumentException>()
            .ParamName.ShouldBe("Email");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongEmail()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.Email = new string('a', 70) + "@b.co"; // Exceeds max length of 69
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void BeCreatedWithValidEmail()
        {
            var user = new UserBuilder().WithEmail(ValidEmail).Build();
            user.Email.ShouldBe(ValidEmail);
        }

        #endregion

        #region FirstName Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeCreatedWithInvalidFirstName(string firstName)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithFirstName(firstName).Build();
            }).ParamName.ShouldBe("FirstName");
        }

        [Fact]
        public void NotBeCreatedWithTooLongFirstName()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithFirstName(new string('a', 101)).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidFirstName(string firstName)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.FirstName = firstName;
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("FirstName");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongFirstName()
        {
            Action act = () =>
           {
               User user = new UserBuilder().Build();
               user.FirstName = new string('a', 101);
           };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void BeCreatedWithValidFirstName()
        {
            var user = new UserBuilder().WithFirstName(ValidFirstName).Build();
            user.FirstName.ShouldBe(ValidFirstName);
        }

        #endregion

        #region FamilyName Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeCreatedWithInvalidFamilyName(string familyName)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithFamilyName(familyName).Build();
            }).ParamName.ShouldBe("FamilyName");
        }

        [Fact]
        public void NotBeCreatedWithTooLongFamilyName()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithFamilyName(new string('a', 101)).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidFamilyName(string familyName)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.FamilyName = familyName;
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("FamilyName");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongFamilyName()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.FamilyName = new string('a', 101); // Exceeds max length of 100
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void BeCreatedWithValidFamilyName()
        {
            var user = new UserBuilder().WithFamilyName(ValidFamilyName).Build();
            user.FamilyName.ShouldBe(ValidFamilyName);
        }

        #endregion

        #region PhoneNumber Tests

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-phone")]
        public void NotBeCreatedWithInvalidPhoneNumber(string phoneNumber)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithPhoneNumber(phoneNumber).Build();
            }).ParamName.ShouldBe("PhoneNumber");
        }

        [Fact]
        public void NotBeCreatedWithTooLongPhoneNumber()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var user = new UserBuilder().WithPhoneNumber(new string('1', 101)).Build();
            }).ParamName.ShouldBe("maxLength");
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("invalid-phone-number")]
        public void NotBeChangedToHaveInvalidPhoneNumber(string phoneNumber)
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.PhoneNumber = phoneNumber;
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("PhoneNumber");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongPhoneNumber()
        {
            Action act = () =>
            {
                User user = new UserBuilder().Build();
                user.PhoneNumber = new string('1', 101); // Exceeds max length of 100
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void BeCreatedWithValidPhoneNumber()
        {
            var user = new UserBuilder().WithPhoneNumber(ValidPhoneNumber).Build();
            user.PhoneNumber.ShouldBe(ValidPhoneNumber);
        }

        #endregion

        #region Address Tests

        [Fact]
        public void BeCreatedWithValidAddress()
        {
            var user = new UserBuilder().WithAddress(ValidAddress).Build();
            user.Address.ShouldBe(ValidAddress);
        }

        [Fact]
        public void NotBeCreatedWithNullAddress()
        {
            Action act = () =>
           {
               var user = new User()
               {
                   Email = ValidEmail,
                   FamilyName = ValidFamilyName,
                   FirstName = ValidFirstName,
                   PhoneNumber = ValidPhoneNumber,
                   DateOfBirth = ValidDateOfBirth,
                   Address = null!,
               };
           };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("Address");
        }

        #endregion

        #region Trimming Tests

        [Fact]
        public void TrimEmail()
        {
            var emailWithSpaces = $"   {ValidEmail}   ";
            var user = new UserBuilder().WithEmail(emailWithSpaces).Build();
            user.Email.ShouldBe(ValidEmail); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimFirstName()
        {
            var firstNameWithSpaces = $"   {ValidFirstName}   ";
            var user = new UserBuilder().WithFirstName(firstNameWithSpaces).Build();
            user.FirstName.ShouldBe(ValidFirstName); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimFamilyName()
        {
            var familyNameWithSpaces = $"   {ValidFamilyName}   ";
            var user = new UserBuilder().WithFamilyName(familyNameWithSpaces).Build();
            user.FamilyName.ShouldBe(ValidFamilyName); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimPhoneNumber()
        {
            var phoneNumberWithSpaces = $"   {ValidPhoneNumber}   ";
            var user = new UserBuilder().WithPhoneNumber(phoneNumberWithSpaces).Build();
            user.PhoneNumber.ShouldBe(ValidPhoneNumber); // Assert that whitespace is trimmed
        }

        #endregion

        #region Reservations and Batteries

        [Fact]
        public void HaveEmptyReservationsAndBatteriesUponCreation()
        {
            var user = new UserBuilder().Build();
            user.Reservations.ShouldBeEmpty();
            user.GuardedBatteries.ShouldBeEmpty();
        }

        #endregion
    }
}