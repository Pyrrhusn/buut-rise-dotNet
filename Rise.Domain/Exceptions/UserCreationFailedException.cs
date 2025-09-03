
namespace Rise.Domain.Exceptions;

public class UserCreationFailedException(string message) : ApplicationException(message)
{
}