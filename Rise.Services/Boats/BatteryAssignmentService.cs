using Microsoft.EntityFrameworkCore;
using Rise.Persistence;
using Microsoft.Extensions.Logging;
using Rise.Domain.Boats;
using Rise.Shared.Notifications;
using Rise.Domain.Reservations;

namespace Rise.Services.Boats
{
    public class BatteryAssignmentService(
        ApplicationDbContext dbContext,
        ILogger<BatteryAssignmentService> logger,
        IInternalNotificationService internalNotificationService)
    {
        private readonly ApplicationDbContext _dbContext = dbContext;
        private readonly ILogger<BatteryAssignmentService> _logger = logger;
        private readonly IInternalNotificationService _internalNotificationService = internalNotificationService;

        public async Task AssignAndOptimizeBatteries()
        {
            TimeInfo timeInfo = GetCurrentTimeInfo();

            _logger.LogInformation("Start assigning batteries for reservations oncoming three days");
            var assignedReservations = await AssignBatteriesToReservations(timeInfo);
            _logger.LogInformation("Done assigning batteries for reservations");

            await _dbContext.SaveChangesAsync();

            await _internalNotificationService.SendBatteryNotificationsToUsers(assignedReservations);
        }

        private static TimeInfo GetCurrentTimeInfo()
        {
            var now = DateTime.UtcNow;
            return new TimeInfo(
                Now: now,
                Today: DateOnly.FromDateTime(now),
                CurrentTime: TimeOnly.FromDateTime(now),
                ThreeDaysFromNow: DateOnly.FromDateTime(now).AddDays(3)
            );
        }

        private async Task<List<Reservation>> AssignBatteriesToReservations(TimeInfo timeInfo)
        {
            List<Boat> boats = await _dbContext.Boats
                .Include(boat => boat.Reservations
                    .Where(
                    res =>
                    res.Battery! == null! &&
                    timeInfo.Today <= res.TimeSlot.Date ||
                        res.TimeSlot.Date <= timeInfo.ThreeDaysFromNow
                    )
                    .OrderBy(res => res.TimeSlot.Date)
                    .ThenBy(res => res.TimeSlot.Start)
                )
                    .ThenInclude(res => res.TimeSlot)
                .Include(boat => boat.Reservations)
                    .ThenInclude(res => res.Battery)
                        .ThenInclude(bat => bat!.Mentor)
                .Include(boat => boat.Reservations)
                    .ThenInclude(res => res.Battery)
                        .ThenInclude(bat => bat!.Reservations.Where(
                            res => res.TimeSlot.Date <= timeInfo.Today
                            ))
                            .ThenInclude(res => res.TimeSlot)
                .Include(boat => boat.Reservations)
                    .ThenInclude(res => res.PreviousBatteryHolder)
                .Include(boat => boat.Reservations)
                    .ThenInclude(res => res.User)
                .Include(boat => boat.Batteries)
                .ToListAsync();

            return boats
                .SelectMany(b => b.AssignBatteriesToReservations(timeInfo.Now))
                .ToList();
        }
    }

    internal record TimeInfo(
    DateTime Now,
    DateOnly Today,
    TimeOnly CurrentTime,
    DateOnly ThreeDaysFromNow);
}
