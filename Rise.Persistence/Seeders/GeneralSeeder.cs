using Microsoft.EntityFrameworkCore;

namespace Rise.Persistence.Seeders
{
    /// <summary>
    /// General template for seeding an Entity
    /// </summary>
    /// <typeparam name="T">the entity in the database</typeparam>
    /// <param name="dbContext">Database context to perform seeding with/on</param>
    internal abstract class GeneralSeeder<T>(ApplicationDbContext dbContext) where T : class
    {

        protected readonly ApplicationDbContext _dbContext = dbContext;

        protected abstract IEnumerable<T> Items { get; }
        protected abstract DbSet<T> DbSet { get; }

        public void Seed()
        {
            if (!HasAlreadyBeenSeeded())
            {
                PersistItemsToDatabase();
            }
        }

        /// <returns>If the database already has been seeded with record for the entity</returns>
        protected virtual bool HasAlreadyBeenSeeded()
        {
            return DbSet.Any();
        }

        /// <summary>Adds the seeding records to the database</summary>

        protected void PersistItemsToDatabase()
        {
            DbSet.AddRange(Items);
            _dbContext.SaveChanges();
        }
    }
}