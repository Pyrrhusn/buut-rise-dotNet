namespace Rise.Shared.Users;

public interface IUserRegisterService
{
    Task<int> RegisterUser(UserRegistrationModelDto userDto);
}
