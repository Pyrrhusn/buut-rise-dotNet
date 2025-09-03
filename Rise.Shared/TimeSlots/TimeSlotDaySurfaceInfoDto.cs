namespace Rise.Shared.TimeSlots
{
    /// <summary>
    /// The surface info on the reservation state of a day
    /// </summary>
    /// <param name="Date">The date</param>
    /// <param name="IsFullyBooked">If the day is fully booked</param>
    /// <param name="IsSlotAvailable">If the day has a slot available to be booked</param>
    /// <param name="IsBookedByUser">If the day is booked by the current user</param>
    public record class TimeSlotDaySurfaceInfoDto(DateOnly Date, bool IsFullyBooked, bool IsSlotAvailable, bool IsBookedByUser = false)
    {

    }
}