using Rise.Domain.Users;
using Shouldly;

namespace Rise.Domain.Tests.Users
{
    public class UserAddressShould
    {
        private static readonly User.UserAddress ValidAddress = new()
        {
            Street = "123 Main St.",
            Number = "4B",
            City = "Springfield",
            PostalCode = "12345",
            Country = "USA"
        };

        [Theory]
        [InlineData("", "123", "Springfield", "12345", "USA")] // Empty Street
        [InlineData("   ", "123", "Springfield", "12345", "USA")] // Whitespace Street
        [InlineData("123", "", "Springfield", "12345", "USA")] // Empty Number
        [InlineData("123", "   ", "Springfield", "12345", "USA")] // Whitespace Number
        [InlineData("123", "123", "", "12345", "USA")] // Empty City
        [InlineData("123", "123", "   ", "12345", "USA")] // Whitespace City
        [InlineData("123", "123", "Springfield", "", "USA")] // Empty PostalCode
        [InlineData("123", "123", "Springfield", "   ", "USA")] // Whitespace PostalCode
        [InlineData("123", "123", "Springfield", "12345", "")] // Empty Country
        [InlineData("123", "123", "Springfield", "12345", "   ")] // Whitespace Country
        public void NotBeCreatedWithInvalidAddress(
            string street, string number, string city, string postalCode, string country)
        {
            Should.Throw<ArgumentException>(() =>
            {
                var invalidAddress = new User.UserAddress
                {
                    Street = street,
                    Number = number,
                    City = city,
                    PostalCode = postalCode,
                    Country = country
                };
            });
        }

        #region "Street"

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidStreet(string street)
        {
            Action act = () =>
            {
                var address = new User.UserAddress
                {
                    Street = street,
                    Number = "123",
                    City = "City",
                    PostalCode = "12345",
                    Country = "Country"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("Street");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongStreet()
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = new string('S', 201), // Exceeds max length of 200
                    Number = "123",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongStreet()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = new string('S', 201),
                    Number = "123",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                };
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region Number

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidNumber(string number)
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = "Street",
                    Number = number,
                    City = "City",
                    PostalCode = "12345",
                    Country = "Country"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("Number");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongNumber()
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = new string('N', 201), // Exceeds max length of 200
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongNumber()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = new string('N', 201),
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = "USA"
                };
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region City

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidCity(string city)
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = "Street",
                    Number = "123",
                    City = city,
                    PostalCode = "12345",
                    Country = "Country"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("City");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongCity()
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = new string('C', 201), // Exceeds max length of 200
                    PostalCode = "12345",
                    Country = "USA"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongCity()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = new string('C', 201),
                    PostalCode = "12345",
                    Country = "USA"
                };
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region PostalCode

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidPostalCode(string postalCode)
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = "Street",
                    Number = "123",
                    City = "City",
                    PostalCode = postalCode,
                    Country = "Country"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("PostalCode");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongPostalCode()
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = "Springfield",
                    PostalCode = new string('P', 101), // Exceeds max length of 100
                    Country = "USA"
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongPostalCode()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = "Springfield",
                    PostalCode = new string('P', 101),
                    Country = "USA"
                };
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region Country

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void NotBeChangedToHaveInvalidCountry(string country)
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = "Street",
                    Number = "123",
                    City = "City",
                    PostalCode = "12345",
                    Country = country
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("Country");
        }

        [Fact]
        public void NotBeChangedToHaveTooLongCountry()
        {
            Action act = () =>
            {

                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = new string('C', 101) // Exceeds max length of 100
                };
            };

            act.ShouldThrow<ArgumentException>()
                .ParamName.ShouldBe("maxLength");
        }

        [Fact]
        public void NotBeCreatedWithTooLongCountry()
        {
            Should.Throw<ArgumentException>(() =>
            {
                var address = new User.UserAddress
                {
                    Street = "123 Main St",
                    Number = "123",
                    City = "Springfield",
                    PostalCode = "12345",
                    Country = new string('C', 101)
                };
            }).ParamName.ShouldBe("maxLength");
        }

        #endregion

        #region Valid address

        [Fact]
        public void BeCreatedWithValidData()
        {
            var address = new User.UserAddress()
            {
                Street = ValidAddress.Street,
                Number = ValidAddress.Number,
                City = ValidAddress.City,
                PostalCode = ValidAddress.PostalCode,
                Country = ValidAddress.Country
            };
            address.Street.ShouldBe(ValidAddress.Street);
            address.Number.ShouldBe(ValidAddress.Number);
            address.City.ShouldBe(ValidAddress.City);
            address.PostalCode.ShouldBe(ValidAddress.PostalCode);
            address.Country.ShouldBe(ValidAddress.Country);
        }

        #endregion

        #region Trim tests

        [Fact]
        public void TrimAddressStreet()
        {
            var untrimmedStreet = $"   {ValidAddress.Street}   ";

            var address = new User.UserAddress
            {
                Street = untrimmedStreet,
                Number = ValidAddress.Number,
                City = ValidAddress.City,
                PostalCode = ValidAddress.PostalCode,
                Country = ValidAddress.Country
            };

            address.Street.ShouldBe(ValidAddress.Street); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimAddressNumber()
        {
            var untrimmedNumber = $"   {ValidAddress.Number}   ";

            var address = new User.UserAddress
            {
                Street = ValidAddress.Street,
                Number = untrimmedNumber,
                City = ValidAddress.City,
                PostalCode = ValidAddress.PostalCode,
                Country = ValidAddress.Country
            };

            address.Number.ShouldBe(ValidAddress.Number);
        }

        [Fact]
        public void TrimAddressCity()
        {
            var untrimmedCity = $"   {ValidAddress.City}   ";

            var address = new User.UserAddress
            {
                Street = ValidAddress.Street,
                Number = ValidAddress.Number,
                City = untrimmedCity,
                PostalCode = ValidAddress.PostalCode,
                Country = ValidAddress.Country
            };

            address.City.ShouldBe(ValidAddress.City); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimAddressPostalCode()
        {
            var untrimmedPostalCode = $"   {ValidAddress.PostalCode}   ";

            var address = new User.UserAddress
            {
                Street = ValidAddress.Street,
                Number = ValidAddress.Number,
                City = ValidAddress.City,
                PostalCode = untrimmedPostalCode,
                Country = ValidAddress.Country
            };

            address.PostalCode.ShouldBe(ValidAddress.PostalCode); // Assert that whitespace is trimmed
        }

        [Fact]
        public void TrimAddressCountry()
        {
            var untrimmedCountry = $"   {ValidAddress.Country}   ";

            var address = new User.UserAddress
            {
                Street = ValidAddress.Street,
                Number = ValidAddress.Number,
                City = ValidAddress.City,
                PostalCode = ValidAddress.PostalCode,
                Country = untrimmedCountry
            };

            address.Country.ShouldBe(ValidAddress.Country); // Assert that whitespace is trimmed
        }

        #endregion
    }
}