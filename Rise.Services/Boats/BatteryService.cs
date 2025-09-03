using Rise.Shared.Boats;
using Rise.Services.Auth;
using Rise.Persistence;
using Rise.Domain.Users;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Boats;
using Rise.Domain.Exceptions;
using Rise.Shared.Users;

namespace Rise.Services.Boats
{
    public class BatteryService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
        : AuthenticatedService(dbContext, authContextProvider), IBatteryService
    {
        /// <param name="id">battery id</param>
        /// <returns></returns>
        /// <exception cref="EntityNotFoundException">When the battery does not exist with given id</exception>
        public async Task<Battery> FindBattery(int id)
        {
            return await _dbContext.Batteries.Include(b => b.Mentor)
            .FirstOrDefaultAsync(b => b.Id == id) ?? throw new EntityNotFoundException(nameof(Battery), id);
        }

        public async Task<BatteryDto> GetBattery(int id)
        {
            Battery battery = await FindBattery(id);

            return new BatteryDto
            {
                Id = battery.Id,
                Mentor = new()
                {
                    Id = battery.Mentor.Id,
                    FirstName = battery.Mentor.FirstName,
                    FamilyName = battery.Mentor.FamilyName,
                    FullName = battery.Mentor.FullName,
                },
                Type = battery.Type
            };
        }

        public async Task<BatteryDto> UpdateBattery(int id, BatteryUpdateDto newBattery)
        {
            Battery battery = await FindBattery(id);

            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == newBattery.MentorId) ?? throw new EntityNotFoundException(nameof(User), newBattery.MentorId);

            battery.Mentor = user;
            battery.Type = newBattery.Type;

            await _dbContext.SaveChangesAsync();

            return new BatteryDto
            {
                Id = battery.Id,
                Mentor = new()
                {
                    Id = battery.Mentor.Id,
                    FirstName = battery.Mentor.FirstName,
                    FamilyName = battery.Mentor.FamilyName,
                    FullName = battery.Mentor.FullName,
                },
                Type = battery.Type
            };
        }

        public async Task<IEnumerable<BatteryDto>> GetBatteriesByBoat(int boatId)
        {
            var boat = await _dbContext.Boats.FindAsync(boatId)
                ?? throw new EntityNotFoundException(nameof(Boat), boatId);

            var batteries = await _dbContext.Batteries
                .Include(b => b.Mentor)
                .Where(b => b.Boat.Id == boatId)
                .Select(battery => new BatteryDto
                {
                    Id = battery.Id,
                    Type = battery.Type,
                    Mentor = new UserNameDto
                    {
                        Id = battery.Mentor.Id,
                        FirstName = battery.Mentor.FirstName,
                        FamilyName = battery.Mentor.FamilyName,
                        FullName = battery.Mentor.FullName
                    }
                })
                .ToListAsync();

            return batteries;
        }


    }
}
