using System;

namespace Rise.Shared.Users;

public record UserDto : BaseDto
{
    public required string FamilyName { get; set; }
}
