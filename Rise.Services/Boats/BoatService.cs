using System;
using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Rise.Services.Auth;
using Rise.Shared.Boats;
using Rise.Shared;
using Rise.Domain.Exceptions;
using Rise.Domain.Boats;
using Rise.Shared.Users;

namespace Rise.Services.Boats
{
    public class BoatService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
         : AuthenticatedService(dbContext, authContextProvider), IBoatService
    {


        /// <summary>
        /// get the count of active boats
        /// </summary>
        /// <returns>the count of active boats </returns>        
        public async Task<int> GetActiveBoatsCountAsync()
        {
            return await _dbContext.Boats
                .CountAsync(b => !b.IsDeleted);
        }

        public async Task<IEnumerable<BoatDto>> GetAllBoatsAsync()
        {
            var boats = await _dbContext.Boats
            .OrderBy(boat => !boat.IsAvailable)
            .ThenBy(boat => boat.PersonalName)
            .Select(boat => new BoatDto
            {
                Id = boat.Id,
                PersonalName = boat.PersonalName,
                IsAvailable = boat.IsAvailable
            })
            .ToListAsync();

            return boats;
        }


        /// <summary>
        /// Updates the availability of a specific boat by its ID.
        /// </summary>
        /// <param name="boatId">The ID of the boat to update.</param>
        /// <param name="isAvailable">The new availability status of the boat.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        public async Task UpdateBoatAvailabilityAsync(int boatId, bool isAvailable)
        {
            Boat boat = await _dbContext.Boats.FindAsync(boatId)
                ?? throw new EntityNotFoundException(nameof(Boat), boatId);

            boat.ChangeAvailability(isAvailable);

            if (!isAvailable)
            {
                await CancelReservationsForBoat(boat.Id);
            }

            await _dbContext.SaveChangesAsync();
        }

        private async Task CancelReservationsForBoat(int boatId)
        {
            var reservations = await _dbContext.Reservations
               .Include(r => r.TimeSlot)
               .Where(r => r.BoatId == boatId && r.TimeSlot.Date >= DateOnly.FromDateTime(DateTime.Now) && !r.IsDeleted)
               .ToListAsync();

            bool isAdmin = _authContextProvider.IsAdmin();
            reservations.ForEach(reservation => reservation.Cancel(isAdmin));
        }

        public async Task<int> CreateBoatAsync(CreateBoatDto createBoatDto)
        {
            Boat boat = new()
            {
                PersonalName = createBoatDto.PersonalName,
                IsAvailable = createBoatDto.IsAvailable,
            };

            await _dbContext.Boats.AddAsync(boat);

            _dbContext.SaveChanges();

            return boat.Id;
        }



    }

}

