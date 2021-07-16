using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Location
{
    public interface ILocationMemoryCache
    {
        public static IReadOnlyDictionary<int, LocationCountryContract> LocationCountries => LocationMemoryCache.LocationCountries;
        public static IReadOnlyDictionary<int, LocationStateContract> LocationStates => LocationMemoryCache.LocationStates;
        public static IReadOnlyDictionary<int, LocationCityContract> LocationCities => LocationMemoryCache.LocationCities;
    }

    static class LocationMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, LocationCountryContract> LocationCountries = new();
        public static readonly ConcurrentDictionary<int, LocationStateContract> LocationStates = new();
        public static readonly ConcurrentDictionary<int, LocationCityContract> LocationCities = new();

        public static async Task LoadAsync(LocationRepository repository)
        {
            if (IsReady) return;

            var countries = (await repository.GetCountriesAsync()).Select(countryEntity => countryEntity.MapToCountryContract()).ToList();
            var states = (await repository.GetStatesAsync()).Select(stateEntity => stateEntity.MapToStateContract()).ToList();
            var cities = (await repository.GetCitiesAsync()).Select(cityEntity => cityEntity.MapToCityContract()).ToList();

            foreach (var country in countries)
                LocationCountries[country.CountryId] = country;

            foreach (var state in states)
                LocationStates[state.StateId] = state;

            foreach (var city in cities)
                LocationCities[city.CityId] = city;

            IsReady = true;
        }
    }
}