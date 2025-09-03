using Rise.Services.Boats;

namespace Rise.Server.Workers
{
    public class BatteryAssignmentWorker(
        IServiceProvider services,
        ILogger<BatteryAssignmentWorker> logger) : BackgroundService
    {
        private readonly IServiceProvider _services = services;
        private readonly ILogger<BatteryAssignmentWorker> _logger = logger;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _services.CreateScope();
                    var batteryService = scope.ServiceProvider.GetRequiredService<BatteryAssignmentService>();

                    await batteryService.AssignAndOptimizeBatteries();
                    
                    // Run every 2 hour
                    await Task.Delay(TimeSpan.FromHours(2), stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while assigning batteries");
                    await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                }
            }
        }

    }
}
