global using Library.Base;
global using Library.Location.Contract;
global using Library.Location.Entity;
global using Library.Location.Job;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Location
{
    public interface ILocationStartup
    {
        public static void Startup(IServiceCollection services, LocationConfiguration configuration, string databaseConnection) =>
            LocationStartup.Startup(services, configuration, databaseConnection);

        public static object GetStatus() =>
            LocationStartup.GetStatus();
    }

    static class LocationStartup
    {
        public static bool IsReady { get; private set; }

        public static void Startup(IServiceCollection services, LocationConfiguration configuration, string databaseConnection)
        {
            if (IsReady) return;

            services
                //Internal
                .AddDbContext<LocationContext>(options => options.UseSqlServer(databaseConnection, sqlServerOptions => sqlServerOptions.UseNetTopologySuite()))
                .AddTransient<LocationRepository>()
                .AddTransient<LocationService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<ILocationRepository, LocationRepository>()
                .AddTransient<ILocationService, LocationService>()
                //Job
                .AddHostedService<LocationProcessMemoryCacheJob>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            IsMemoryCacheReady = LocationMemoryCache.IsReady,
            RegisteredTypes = new
            {
                Internal = new[]
                {
                    nameof(LocationContext),
                    nameof(LocationRepository),
                    nameof(LocationService),
                    nameof(LocationMemoryCache),
                    nameof(LocationConfiguration)
                },
                Public = new[]
                {
                    nameof(ILocationRepository),
                    nameof(ILocationService),
                    nameof(ILocationMemoryCache)
                },
                Job = new[]
                {
                    nameof(LocationProcessMemoryCacheJob)
                }
            },
            MemoryCache = new
            {
                TotalCountries = LocationMemoryCache.LocationCountries.Count,
                TotalStates = LocationMemoryCache.LocationStates.Count,
                TotalCities = LocationMemoryCache.LocationCities.Count
            }
        };
    }
}