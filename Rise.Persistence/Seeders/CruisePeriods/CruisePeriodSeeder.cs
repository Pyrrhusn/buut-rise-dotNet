using Microsoft.EntityFrameworkCore;
using Rise.Domain.TimeSlots;

namespace Rise.Persistence.Seeders.CruisePeriods;

internal class CruisePeriodSeeder(ApplicationDbContext dbContext) : GeneralSeeder<CruisePeriod>(dbContext)
{
    /// <summary>
    /// Cruise period that started one month ago and ends until two days before today.
    /// </summary>
    public readonly CruisePeriod PastMonthLong = new() { Start = DateTime.Today.AddMonths(-1), End = DateTime.Today.AddDays(-2).AddHours(23) };

    /// <summary>
    /// Cruise period that starts now for a full week.
    /// </summary>
    public readonly CruisePeriod WeekLong = new() { Start = DateTime.Today, End = DateTime.Today.AddDays(7).AddHours(23) };

    /// <summary>
    /// Cruise period that starts nine days from now for two weeks and 2 days. 
    /// </summary>
    public readonly CruisePeriod TwoWeekAndTwoDaysLong = new() { Start = DateTime.Today.AddDays(9), End = DateTime.Today.AddDays(25).AddHours(23) };

    /// <summary>
    /// Cruise period that starts next month from now for a month. 
    /// </summary>
    public readonly CruisePeriod MonthLong = new() { Start = DateTime.Today.AddMonths(1), End = DateTime.Today.AddMonths(2).AddHours(23) };

    protected override DbSet<CruisePeriod> DbSet => _dbContext.CruisePeriods;
    protected override IEnumerable<CruisePeriod> Items => [
        PastMonthLong,
        WeekLong,
        TwoWeekAndTwoDaysLong,
        MonthLong,
    ];
}