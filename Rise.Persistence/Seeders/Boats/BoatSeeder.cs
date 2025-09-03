using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;

namespace Rise.Persistence.Seeders.Boats;
internal class BoatSeeder(ApplicationDbContext dbContext) : GeneralSeeder<Boat>(dbContext)
{

    public readonly Boat Limba = new() { PersonalName = "Limba", IsAvailable = true };
    public readonly Boat Leith = new() { PersonalName = "Leith", IsAvailable = true };
    public readonly Boat Lubeck = new() { PersonalName = "Lubeck", IsAvailable = true };

    protected override DbSet<Boat> DbSet => _dbContext.Boats;

    protected override IEnumerable<Boat> Items => [Limba, Leith, Lubeck];
}
