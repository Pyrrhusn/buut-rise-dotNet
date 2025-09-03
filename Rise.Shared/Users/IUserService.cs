using System;

namespace Rise.Shared.Users;

public interface IUserService
{
    Task<UserProfileDto> GetUserProfile();
    Task UpdateUserAsync(UpdateUserProfileDto updateUserProfileDto);
}
