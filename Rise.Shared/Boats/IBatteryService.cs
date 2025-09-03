namespace Rise.Shared.Boats
{
    public interface IBatteryService
    {
        /// <summary>
        /// Get info about battery
        /// </summary>
        /// <param name="id">battery id</param>
        /// <returns>battery</returns>
        Task<BatteryDto> GetBattery(int id);

        /// <summary>
        /// Update the battery with new info
        /// </summary>
        /// <param name="id">battery id</param>
        /// <param name="newBattery">new battery info</param>
        /// <returns>The new version of the battery</returns>
        /// <exception cref="EntityNotFoundException">When the battery or mentor could not be found</exception>
        Task<BatteryDto> UpdateBattery(int id, BatteryUpdateDto newBattery);


        /// <summary>
        /// Get all batteries for a boat
        ///</summary>
        /// <param name="boatId">The ID of the boat to get batteries for.</param>
        /// <returns>The batteries for the boat.</returns>
        Task<IEnumerable<BatteryDto>> GetBatteriesByBoat(int boatId);

    }

}