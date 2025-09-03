namespace Rise.Domain.Exceptions;

/// <summary>
/// <see cref="Exception"/> to throw when the app_metadata of the user is not what we expect.
/// </summary>
public class UserInvalidClaimStructureException : ApplicationException
{
    public UserInvalidClaimStructureException() : base($"The token has an invalid claim structure")
    {

    }
}
