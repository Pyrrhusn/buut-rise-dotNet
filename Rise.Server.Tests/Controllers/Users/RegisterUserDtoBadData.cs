using System;
using System.Collections;
using Rise.Shared.Users;

namespace Rise.Server.Tests.Controllers.Users;

public class RegisterUserDtoBadData : IEnumerable<object[]>
{
    //Powered by ChatGPT
    public IEnumerator<object[]> GetEnumerator()
    {
        // Valid data to use as a baseline
        var validData = new UserRegistrationModelDto
        {
            Email = "john.doe@example.com",
            FamilyName = "Doe",
            FirstName = "John",
            Password = "SecureP@ssw0rd",
            PhoneNumber = "+123456789",
            DateOfBirth = new DateTime(2005, 1, 1),
            Address = new()
            {
                City = "Gent",
                Country = "België",
                Number = "10",
                PostalCode = "9000",
                Street = "Fabiolalaan",
            }
        };
        // Invalid Email: Empty
        yield return new object[]
        {
            validData with { Email = "" }
        };

        // Invalid Email: Exceeds max length
        yield return new object[]
        {
            validData with { Email = new string('a', 101) + "@example.com" }
        };

        // Invalid Email: Invalid format
        yield return new object[]
        {
            validData with { Email = "invalid-email" }
        };

        // Invalid FirstName: Empty
        yield return new object[]
        {
            validData with { FirstName = "" }
        };

        // Invalid FirstName: Exceeds max length
        yield return new object[]
        {
            validData with { FirstName = new string('a', 101) }
        };

        // Invalid FamilyName: Empty
        yield return new object[]
        {
            validData with { FamilyName = "" }
        };

        // Invalid FamilyName: Exceeds max length
        yield return new object[]
        {
            validData with { FamilyName = new string('a', 101) }
        };

        // Invalid Password: Empty
        yield return new object[]
        {
            validData with { Password = "" }
        };

        // Invalid PhoneNumber: Empty
        yield return new object[]
        {
            validData with { PhoneNumber = "" }
        };

        // Invalid PhoneNumber: Exceeds max length
        yield return new object[]
        {
            validData with { PhoneNumber = new string('1', 101) }
        };

        // Invalid PhoneNumber: Invalid format
        yield return new object[]
        {
            validData with { PhoneNumber = "InvalidPhone" }
        };

        // Invalid Address: Empty Street
        yield return new object[]
        {
            validData with { Address = validData.Address with { Street = "" } }
        };

        // Invalid Address: Street exceeds max length
        yield return new object[]
        {
            validData with { Address = validData.Address with { Street = new string('a', 201) } }
        };

        // Invalid Address: Empty Number
        yield return new object[]
        {
            validData with { Address = validData.Address with { Number = "" } }
        };

        // Invalid Address: Number exceeds max length
        yield return new object[]
        {
            validData with { Address = validData.Address with { Number = new string('1', 201) } }
        };

        // Invalid Address: Empty City
        yield return new object[]
        {
            validData with { Address = validData.Address with { City = "" } }
        };

        // Invalid Address: City exceeds max length
        yield return new object[]
        {
            validData with { Address = validData.Address with { City = new string('a', 201) } }
        };

        // Invalid Address: Empty PostalCode
        yield return new object[]
        {
            validData with { Address = validData.Address with { PostalCode = "" } }
        };

        // Invalid Address: PostalCode exceeds max length
        yield return new object[]
        {
            validData with { Address = validData.Address with { PostalCode = new string('1', 101) } }
        };

        // Invalid Address: Empty Country
        yield return new object[]
        {
            validData with { Address = validData.Address with { Country = "" } }
        };

        // Invalid Address: Country exceeds max length
        yield return new object[]
        {
            validData with { Address = validData.Address with { Country = new string('a', 101) } }
        };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
