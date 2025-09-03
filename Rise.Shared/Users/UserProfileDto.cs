using System;
using Rise.Shared.Address;

namespace Rise.Shared.Users;

public record class UserProfileDto
{
    public required string Email { get; set; }
    public required string FirstName { get; set; }
    public required string FamilyName { get; set; }
    public required string PhoneNumber { get; set; }
    public required AddressDto Address { get; set; }
    public required DateTime DateOfBirth { get; set; }
}
