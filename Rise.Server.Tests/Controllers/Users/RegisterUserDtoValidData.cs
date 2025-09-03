using System;
using System.Collections;
using Rise.Shared.Users;
using static Rise.Shared.Users.UserRegistrationModelDto;

namespace Rise.Server.Tests.Controllers.Users;

public class RegisterUserDtoValidData : IEnumerable<object[]>
{
    //Powered by ChatGPT
    public IEnumerator<object[]> GetEnumerator()
    {
        // Edge cases for valid data
        yield return new object[]
       {
            new UserRegistrationModelDto
            {
                Email = "a@d.co", // Minimal valid email
                Password = string.Concat(Enumerable.Repeat("pA0!", 16)), // Max-length password
                FirstName = new string('F', 100), // Max-length first name
                FamilyName = new string('L', 100), // Max-length family name
                PhoneNumber = "+1234567890", // Valid phone number
                DateOfBirth = new DateTime(2005, 1, 1),
                Address = new AddressModel
                {
                    Street = new string('S', 200), // Max-length street name
                    Number = new string('N', 25), // Max-length house number
                    City = new string('C', 200), // Max-length city name
                    PostalCode = new string('9', 100), // Max-length postal code
                    Country = new string('X', 100) // Max-length country name
                }
            }
       };

        yield return new object[]
        {
            new UserRegistrationModelDto(){
                Email= string.Concat(["a.",new string('a', 62),"@d.co"]) , // Max-length email
                Password= "P@ssw0rd123", // Complex valid password
                FirstName= "Anne-Marie", // Name with a hyphen
                FamilyName= "O'Connor", // Name with an apostrophe
                PhoneNumber= "(123) 456-7890", // Valid US-style phone number
                DateOfBirth = new DateTime(2005, 1, 1),
                Address= new AddressModel(){
                    Street= "123 Main St. Apt. 4B", // Street with extra details
                    Number= "4B", // Alphanumeric house number
                    City= "Ålesund", // City with a special character
                    PostalCode= "5000", // Valid numeric postal code
                    Country= "Norge" // Country name in native language
                }
        }
        };

        yield return new object[]
        {
            new UserRegistrationModelDto
            {
                Email = "aa@d.co",
                Password = "SecurePassword123!", // Complex valid password
                FirstName = "Zoë", // Name with special character
                FamilyName = "D'Angelo", // Name with apostrophe
                PhoneNumber = "911",
                DateOfBirth = new DateTime(2005, 1, 1),
                Address = new AddressModel
                {
                    Street = "Via Roma 123", // Non-English street name
                    Number = "123", // Standard house number
                    City = "München", // City with umlaut
                    PostalCode = "D-80333", // German postal code format
                    Country = "Deutschland" // Non-English country name
                }
            }
        };

        yield return new object[]{
            new UserRegistrationModelDto()
            {
                Email = "john.doe@test.com",
                Password = "SecureP@ssw0rd123",
                FirstName = "John",
                FamilyName = "Doe",
                PhoneNumber = "+32471123456",
                DateOfBirth = new DateTime(2005, 1, 1),
                Address = new()
                {
                    Street = "Fabiolalaan",
                    Number = "10",
                    City = "Gent",
                    PostalCode = "9000",
                    Country = "Belgium"
                }
            }
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
