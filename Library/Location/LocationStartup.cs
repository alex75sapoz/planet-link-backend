using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Location
{
    public interface ILocationStartup
    {
        public static bool IsMemoryCacheReady =>
            LocationStartup.IsMemoryCacheReady;

        public static void Startup(IServiceCollection services, Func<Type, object> configuration, string databaseConnection) =>
            LocationStartup.Startup(services, (LocationConfiguration)configuration(typeof(LocationConfiguration)), databaseConnection);

        public static async Task RefreshMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await LocationStartup.RefreshMemoryCacheAsync(serviceProvider.GetRequiredService<LocationRepository>());

        public static object GetStatus() =>
            LocationStartup.GetStatus();
    }

    internal static class LocationStartup
    {
        public static bool IsStarted { get; set; }
        public static bool IsMemoryCacheReady { get; set; }

        public static void Startup(IServiceCollection services, LocationConfiguration configuration, string databaseConnection)
        {
            IsStarted = false;

            services
                //Internal
                .AddDbContext<LocationContext>(options => options.UseSqlServer(databaseConnection, sqlServerOptions => sqlServerOptions.UseNetTopologySuite()))
                .AddTransient<LocationRepository>()
                .AddTransient<LocationService>()
                .AddSingleton(configuration)
                //Public
                .AddTransient<ILocationRepository, LocationRepository>()
                .AddTransient<ILocationService, LocationService>();

            IsStarted = true;
        }

        public static async Task RefreshMemoryCacheAsync(LocationRepository repository)
        {
            IsMemoryCacheReady = false;

            var countryEntities = await repository.GetCountriesAsync();
            var stateEntities = await repository.GetStatesAsync();
            var citiesEntities = await repository.GetCitiesAsync();

            LocationMemoryCache.LocationCountries.Clear();
            foreach (var country in countryEntities.Select(countryEntity => countryEntity.MapToCountryContract()))
                LocationMemoryCache.LocationCountries.TryAdd(country.CountryId, country);

            LocationMemoryCache.LocationStates.Clear();
            foreach (var state in stateEntities.Select(stateEntity => stateEntity.MapToStateContract()))
                LocationMemoryCache.LocationStates.TryAdd(state.StateId, state);

            LocationMemoryCache.LocationCities.Clear();
            foreach (var city in citiesEntities.Select(cityEntity => cityEntity.MapToCityContract()))
                LocationMemoryCache.LocationCities.TryAdd(city.CityId, city);

            IsMemoryCacheReady = true;
        }

        public static object GetStatus() => new
        {
            IsStarted,
            IsMemoryCacheReady,
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
                    nameof(ILocationService)
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