using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rise.Persistence;

namespace Rise.Server.Tests.Fixtures
{
    public class ApiWebApplicationFactory : WebApplicationFactory<Program>
    {

        public IConfiguration Configuration { get; private set; } = default!;
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureAppConfiguration(config =>
            {
                Configuration = new ConfigurationBuilder()
                    .AddUserSecrets("d8435739-e257-4e40-b03f-9b9a66bbc18c")
                    .AddEnvironmentVariables("ASPNETCORE_ENVIRONMENT:Testing")
                    .AddEnvironmentVariables()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                    .Build();

                config.AddConfiguration(Configuration);
            });

            // For stubbing services
            builder.ConfigureServices(services =>
            {
                // Database will be made, trust the process

                var serviceProvider = services.BuildServiceProvider();
                TestDatabaseInitializer.Init(serviceProvider);
            }
            );
        }
    }

    /// <summary>
    /// Singleton service to initialize the database once
    /// </summary>
    internal class TestDatabaseInitializer
    {
        private static readonly object _lock = new();
        private static bool _databaseInitialized = false;

        public static void Init(IServiceProvider serviceProvider)
        {
            lock (_lock)
            {
                if (!_databaseInitialized)
                {
                    using (var scope = serviceProvider.CreateScope())
                    {
                        var scopedServices = scope.ServiceProvider;
                        var db = scopedServices.GetRequiredService<ApplicationDbContext>();

                        db.Database.EnsureDeleted();
                        db.Database.EnsureCreated();

                        // Ensure seeding is consistent 
                        new Seeder(db).Seed();
                    }

                    _databaseInitialized = true;
                }
            }
        }
    }
}
