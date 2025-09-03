using System;

namespace Rise.Shared.TimeSlots
{
    public interface ICruisePeriodService
    {
        /// <summary>
        /// Gets cruise period by id
        /// </summary>
        /// <param name="id">cruise period id</param>
        /// <returns>Cruise period in full detail</returns>
        Task<CruisePeriodDetailedDto> GetCruisePeriod(int id);

        /// <summary>
        /// Gets all cruise periods
        ///    - if getFuturePeriods is true, returns all future cruise periods
        ///    - if getFuturePeriods is false, returns all past cruise periods
        /// </summary>
        /// <param name="getFuturePeriods">if true, returns all future cruise periods</param>
        /// <returns>List of cruise periods</returns>
        Task<List<CruisePeriodDetailedDto>> GetCruisePeriods(bool getFuturePeriods);
    }
}
