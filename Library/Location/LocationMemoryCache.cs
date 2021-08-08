using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Location
{
    static class LocationMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, LocationCountryContract> Countries = new();
        public static readonly ConcurrentDictionary<int, LocationStateContract> States = new();
        public static readonly ConcurrentDictionary<int, LocationCityContract> Cities = new();

        public static async Task LoadAsync(LocationRepository repository)
        {
            if (IsReady) return;

            var countries = (await repository.GetCountriesAsync()).Select(countryEntity => countryEntity.MapToCountryContract()).ToList();
            var states = (await repository.GetStatesAsync()).Select(stateEntity => stateEntity.MapToStateContract()).ToList();
            var cities = (await repository.GetCitiesAsync()).Select(cityEntity => cityEntity.MapToCityContract()).ToList();

            foreach (var country in countries)
                Countries[country.CountryId] = country;

            foreach (var state in states)
                States[state.StateId] = state;

            foreach (var city in cities)
                Cities[city.CityId] = city;

            IsReady = true;
        }
    }
}