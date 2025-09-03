using Rise.Domain.Boats;
using Rise.Domain.Users;

namespace Rise.Persistence.Seeders.Boats
{
    /// <summary>
    /// All batteries of a boat
    /// </summary>
    /// <param name="boat">owner of the batteries</param>
    internal class BoatBatteryCollection(Boat boat)
    {
        public readonly List<Battery> batteries = [];

        /// <summary>
        /// Add a battery to the collection
        /// </summary>
        /// <param name="type">type of battery</param>
        /// <param name="mentor">responsible person for battery</param>
        /// <returns></returns>
        public BoatBatteryCollection AddBattery(string type, User mentor)
        {
            batteries.Add(new()
            {
                Boat = boat,
                Type = type,
                Mentor = mentor
            });
            return this;
        }
    }
}
