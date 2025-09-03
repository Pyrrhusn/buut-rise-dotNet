namespace Rise.Domain.Exceptions;

public class RoleAssigningFailedException(string message) : ApplicationException(message)
{
}