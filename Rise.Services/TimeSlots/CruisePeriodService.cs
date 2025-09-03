using Microsoft.EntityFrameworkCore;
using Rise.Domain.Exceptions;
using Rise.Domain.TimeSlots;
using Rise.Persistence;
using Rise.Services.Auth;
using Rise.Shared.TimeSlots;

namespace Rise.Services.TimeSlots
{
    public class CruisePeriodService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
        : AuthenticatedService(dbContext, authContextProvider), ICruisePeriodService
    {
        public async Task<CruisePeriodDetailedDto> GetCruisePeriod(int id)
        {
            CruisePeriod cruisePeriod = await _dbContext.CruisePeriods.FirstOrDefaultAsync(cp => cp.Id == id) ?? throw new EntityNotFoundException(nameof(CruisePeriod), id);

            return new CruisePeriodDetailedDto()
            {
                Id = cruisePeriod.Id,
                Start = cruisePeriod.Start,
                End = cruisePeriod.End
            };
        }

        public async Task<List<CruisePeriodDetailedDto>> GetCruisePeriods(bool getFuturePeriods)
        {
            var today = DateTime.UtcNow.Date;
            var query = _dbContext.CruisePeriods.AsQueryable();

            query = getFuturePeriods
                ? query.Where(cp => cp.End >= today) // include ongoing and future periods
                : query.Where(cp => cp.End < today); // only truly past periods

            var cruisePeriods = await query
                .OrderBy(cp => cp.Start)
                .Select(cp => new CruisePeriodDetailedDto
                {
                    Id = cp.Id,
                    Start = cp.Start,
                    End = cp.End
                })
                .ToListAsync();

            return cruisePeriods;
        }

    }
}
