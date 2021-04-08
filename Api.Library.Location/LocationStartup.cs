using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Library.Location
{
    public interface ILocationStartup
    {
        public static bool IsMemoryCacheReady =>
            LocationStartup.IsMemoryCacheReady;

        public static void Startup(IServiceCollection services, Func<Type, object> configuration, string databaseConnection) =>
            LocationStartup.Startup(services, (LocationConfiguration)configuration(typeof(LocationConfiguration)), databaseConnection);

        public static async Task RefreshMemoryCacheAsync(IServiceProvider serviceProvider) =>
            await LocationStartup.RefreshMemoryCacheAsync(serviceProvider.GetRequiredService<LocationRepository>());
    }

    internal static class LocationStartup
    {
        public static bool IsMemoryCacheReady { get; set; }

        public static void Startup(IServiceCollection services, LocationConfiguration configuration, string databaseConnection) =>
            services
                .AddDbContext<LocationContext>(options => options.UseSqlServer(databaseConnection, sqlServerOptions => sqlServerOptions.UseNetTopologySuite()))
                .AddTransient<LocationRepository>()
                .AddTransient<ILocationRepository, LocationRepository>()
                .AddTransient<ILocationService, LocationService>()
                .AddSingleton(configuration);

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
    }
}