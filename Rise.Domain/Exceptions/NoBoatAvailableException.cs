namespace Rise.Domain.Exceptions;

/// <summary>
/// <see cref="Exception"/> to throw when no entity is available for the requested criteria.
/// </summary>
public class NoBoatAvailableException(int timeSlotId) : ApplicationException($"No boats are available for time slot {timeSlotId}")
{
}