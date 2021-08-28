global using Library.Base;
global using Library.Location.Contract;
global using Library.Location.Entity;
global using Library.Location.Enum;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Library.Location
{
    public interface ILocationStartup
    {
        public static void ConfigureServices(IServiceCollection services, LocationConfiguration configuration, string databaseConnection) =>
            LocationStartup.ConfigureServices(services, configuration, databaseConnection);

        public static object GetStatus() =>
            LocationStartup.GetStatus();
    }

    static class LocationStartup
    {
        public static bool IsReady { get; private set; }

        public static void ConfigureServices(IServiceCollection services, LocationConfiguration configuration, string databaseConnection)
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
                .AddTransient<ILocationService, LocationService>();

            IsReady = true;
        }

        public static object GetStatus() => new
        {
            IsReady,
            RegisteredTypes = new
            {
                Internal = new[]
                {
                    nameof(LocationContext),
                    nameof(LocationRepository),
                    nameof(LocationService),
                    nameof(LocationConfiguration)
                },
                Public = new[]
                {
                    nameof(ILocationRepository),
                    nameof(ILocationMemoryCache),
                    nameof(ILocationService)
                }
            },
            MemoryCache = new
            {
                TotalCountries = ILocationMemoryCache.Countries.Count,
                TotalStates = ILocationMemoryCache.States.Count,
                TotalCities = ILocationMemoryCache.Cities.Count
            }
        };
    }
}