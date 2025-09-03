using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using Rise.Domain.Reservations;
using Rise.Persistence;
using Rise.Shared.TimeSlots;
using System.Data;
using Rise.Domain.Users;
using Rise.Services.Auth;
using System.Text.Json;
using System.Security.Claims;
using Rise.Domain.Exceptions;
using Rise.Domain.TimeSlots;

namespace Rise.Services.TimeSlots
{
    public class TimeSlotService(ApplicationDbContext dbContext, IAuthContextProvider authContextProvider)
        : AuthenticatedService(dbContext, authContextProvider), ITimeSlotService
    {
        internal class DateTimeSlotBoatUse
        {
            /// <summary>
            /// The date of the time slot
            /// </summary>
            public DateOnly Date { get; set; }
            /// <summary>
            /// The start time of the time slot
            /// </summary>
            public TimeOnly Start { get; set; }
            /// <summary>
            /// The amount of boats who already have been reserved
            /// </summary>
            public int UsedBoatCount { get; set; }
        }

        public async Task<TimeSlotRangeInfoDto> GetAllTimeSlotsInRange(DateOnly startDate, DateOnly endDate)
        {
            Dictionary<DateOnly, TimeSlotDaySurfaceInfoDto> daysWithReservation = [];

            List<DateTimeSlotBoatUse> allTimeSlotsDuringRange = await _dbContext.TimeSlots.Where(
                timeSlot => startDate <= timeSlot.Date && timeSlot.Date <= endDate
                && !timeSlot.IsDeleted)
                .Select(timeSlot => new DateTimeSlotBoatUse()
                {
                    Date = timeSlot.Date,
                    Start = timeSlot.Start,
                    UsedBoatCount = _dbContext.Reservations.Where(reservation => reservation.TimeSlotId == timeSlot.Id).Count()
                })
                .ToListAsync();

            if (allTimeSlotsDuringRange.Count != 0)
            {
                int totalAmountBoats = _dbContext.Boats.Count();

                daysWithReservation = allTimeSlotsDuringRange
                    .GroupBy(
                        x => x.Date,
                        (date, subset) => new
                        {
                            Date = date,
                            TotalDayTimeSlotCount = subset.DistinctBy(x => x.Start).Count(),
                            UsedBoatCount = subset.Select(x => x.UsedBoatCount).Sum()
                        }
                    ).Select(x =>
                    {
                        bool isFullyBooked = x.TotalDayTimeSlotCount * totalAmountBoats <= x.UsedBoatCount;
                        bool isSlotAvailable = !isFullyBooked && x.Date.CompareTo(DateOnly.FromDateTime(DateTime.Today).AddDays(Reservation.MinDaysBetweenReservation)) > 0;
                        return new TimeSlotDaySurfaceInfoDto(x.Date, isFullyBooked, isSlotAvailable);
                    }).ToDictionary(x => x.Date);
            }

            int totalDays = 1 + endDate.ToDateTime(TimeOnly.MinValue).Subtract(startDate.ToDateTime(TimeOnly.MinValue)).Days;
            IEnumerable<TimeSlotDaySurfaceInfoDto> days = Enumerable.Range(0, totalDays)
                    .Select(offset =>
                        {
                            DateOnly date = startDate.AddDays(offset);
                            return daysWithReservation.GetValueOrDefault(date, new TimeSlotDaySurfaceInfoDto(date, false, false));
                        });

            return new TimeSlotRangeInfoDto(totalDays, days);

        }


        public async Task<IEnumerable<TimeSlotDto>> GetTimeSlotsByDate(int year, int month, int day)
        {
            int userId = (int)_authContextProvider.GetUserId()!;

            var date = new DateOnly(year, month, day);
            var today = DateOnly.FromDateTime(DateTime.Today);
            var minReservationDate = GetMinReservationDate();

            // Right now we don't keep the information of whether a boat is available or not
            var amountOfAvailableBoats = await _dbContext.Boats.CountAsync();

            var availableTimeSlots = await _dbContext.TimeSlots
                .Where(ts => ts.Date == date)
                .Select(ts => new
                {
                    TimeSlot = ts,
                    ReservationCount = ts.Reservations.Count(),
                    IsBookedByUser = ts.Reservations.Any(r => r.UserId == userId)
                })
                .Where(item =>
                    (date >= today && date <= minReservationDate && item.IsBookedByUser) ||
                    (date > minReservationDate && (item.ReservationCount < amountOfAvailableBoats || item.IsBookedByUser))
                )
                .Select(item => new TimeSlotDto
                {
                    Id = item.TimeSlot.Id,
                    Start = item.TimeSlot.Start,
                    End = item.TimeSlot.End,
                    IsBookedByUser = item.IsBookedByUser
                })
                .OrderBy(item => item.Start)
                .ToListAsync();

            return availableTimeSlots;
        }



        /// <summary>
        /// Calculates the date from which a reservation can be made
        /// </summary>
        public static DateOnly GetMinReservationDate()
        {
            // Calculate the date based on today's date plus the minimum days
            return DateOnly.FromDateTime(DateTime.Today.AddDays(Reservation.MinDaysBetweenReservation));

        }

        /// <summary>
        /// Creates a new time slot
        /// </summary>
        /// <param name="dto">dto with info to create timslot</param>
        /// <returns>Returns the amount of timeslots that were added</returns>
        /// <exception cref="EntityNotFoundException"></exception>
        public async Task<int> CreateTimeSlot(CreateTimeSlotDto addTimeSlotsDto)
        {
            var cruisePeriod = await _dbContext.CruisePeriods.Include(cp => cp.TimeSlots)
                .FirstOrDefaultAsync(cp => cp.Id == addTimeSlotsDto.CruisePeriodId && !cp.IsDeleted)
                ?? throw new EntityNotFoundException(nameof(CruisePeriod), addTimeSlotsDto.CruisePeriodId);

            //add timeslots for the range of the cruise period
            //we add the timeslots in bulk to the cruise period
            // if one fails, don't add any
            addTimeSlotsDto.TimeSlots.ForEach(timeSlot =>
            {
                cruisePeriod.AddTimeSlots(timeSlot.Start, timeSlot.End);
            });


            await _dbContext.SaveChangesAsync();

            return cruisePeriod.TimeSlots.Count;

        }


    }
}
