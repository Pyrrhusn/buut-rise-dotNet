namespace Rise.Shared.Reservations
{
    /// <summary>
    /// Represents a list DateOnly objects on wich there are reservations
    /// </summary>
    /// <param name="Reservations">List of DateOnly objects on wich there are reservations</param>
    public record ReservationsRangeDto(IEnumerable<DateOnly> Reservations)
    {

    }
}


