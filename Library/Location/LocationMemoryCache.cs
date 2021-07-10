using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Location
{
    public interface ILocationMemoryCache
    {
        public static bool IsReady => LocationMemoryCache.IsReady;

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

        public static async Task RefreshAsync(LocationRepository repository)
        {
            var countries = (await repository.GetCountriesAsync()).Select(countryEntity => countryEntity.MapToCountryContract()).ToDictionary(country => country.CountryId);
            var states = (await repository.GetStatesAsync()).Select(stateEntity => stateEntity.MapToStateContract()).ToDictionary(state => state.StateId);
            var cities = (await repository.GetCitiesAsync()).Select(cityEntity => cityEntity.MapToCityContract()).ToDictionary(city => city.CityId);

            //Countries
            foreach (var country in countries)
                LocationCountries[country.Key] = country.Value;

            foreach (var country in LocationCountries.Where(country => !countries.ContainsKey(country.Key)).ToList())
                LocationCountries.TryRemove(country);

            //States
            foreach (var state in states)
                LocationStates[state.Key] = state.Value;

            foreach (var state in LocationStates.Where(state => !states.ContainsKey(state.Key)).ToList())
                LocationStates.TryRemove(state);

            //Cities
            foreach (var city in cities)
                LocationCities[city.Key] = city.Value;

            foreach (var city in LocationCities.Where(city => !cities.ContainsKey(city.Key)).ToList())
                LocationCities.TryRemove(city);

            IsReady = true;
        }
    }
}