namespace Rise.Domain.Exceptions;

/// <summary>
/// <see cref="Exception"/> to throw when the app_metadata of the user is not what we expect.
/// </summary>
public class RoleNotFoundException(string role) : ApplicationException($"Role '{role}' not found.")
{

}
