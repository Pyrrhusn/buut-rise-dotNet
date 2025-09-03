using System;
using static Rise.Shared.Users.UserRegistrationModelDto;

namespace Rise.Shared.Users;

public record UserDetailDto : UserDto
{
    public required string FirstName { get; set; }
    public required string Email { get; set; }
    public required string PhoneNumber { get; set; }
    public required AddressModel Address { get; set; }
}
