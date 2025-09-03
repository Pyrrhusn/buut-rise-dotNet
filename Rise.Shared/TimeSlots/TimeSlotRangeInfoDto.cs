namespace Rise.Shared.TimeSlots
{
    /// <summary>
    /// The surface info on the reservation states of day range
    /// </summary>
    /// <param name="TotalDays">Amount of days in the range</param>
    /// <param name="Days">Reservation states of every day in the range</param>
    public record class TimeSlotRangeInfoDto(int TotalDays, IEnumerable<TimeSlotDaySurfaceInfoDto> Days)
    {
    }
}