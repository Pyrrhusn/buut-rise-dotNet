namespace Rise.Shared.TimeSlots
{
    public interface ITimeSlotService
    {

        /// <summary>
        /// Gets all time slots between given range
        /// </summary>
        /// <param name="startDate">Date from where the range starts</param>
        /// <param name="endDate">Date from where the range ends (inclusive)</param>
        /// <returns>All time slots with their general info</returns>
        Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDate, DateOnly endDate);
        Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(
            int year,
            int month,
            int day);

        Task<int> CreateTimeSlot(CreateTimeSlotDto dto);
    }
}
