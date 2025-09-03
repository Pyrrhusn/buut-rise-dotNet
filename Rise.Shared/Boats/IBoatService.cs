using Rise.Shared.Boats;

namespace Rise.Shared;

public interface IBoatService
{

    Task<int> GetActiveBoatsCountAsync();

    /// <summary>
    /// Haalt een lijst van alle boten op, inclusief hun beschikbaarheid.
    /// </summary>
    /// <returns>Een lijst van BoatDto-objecten.</returns>
    Task<IEnumerable<BoatDto>> GetAllBoatsAsync();

    Task UpdateBoatAvailabilityAsync(int boatId, bool isAvailable);

    Task<int> CreateBoatAsync(CreateBoatDto createBoatDto);

}

