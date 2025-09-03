namespace Rise.Domain.Exceptions;

public class ReservationCreationFailedException(string message) : ApplicationException(message)
{
}
