using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Location
{
    class LocationService : BaseService<LocationConfiguration, LocationRepository>, ILocationService
    {
        public LocationService(LocationConfiguration configuration, LocationRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region Memory Cache

        internal static ConcurrentDictionary<int, LocationCountryContract> _countries = new();
        internal static ConcurrentDictionary<int, LocationStateContract> _states = new();
        internal static ConcurrentDictionary<int, LocationCityContract> _cities = new();

        public async Task MemoryCacheRefreshAsync(LocationDictionary? dictionary = null, int? id = null)
        {
            if (!dictionary.HasValue || dictionary.Value == LocationDictionary.Countries)
            {
                if (!id.HasValue)
                    _countries = new((await _repository.GetCountriesAsync()).Select(countryEntity => countryEntity.MapToCountryContract()).ToDictionary(country => country.CountryId));
                else
                    _countries[id.Value] = (await _repository.GetCountryAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToCountryContract();
            }

            if (!dictionary.HasValue || dictionary.Value == LocationDictionary.States)
            {
                if (!id.HasValue)
                    _states = new((await _repository.GetStatesAsync()).Select(stateEntity => stateEntity.MapToStateContract()).ToDictionary(state => state.StateId));
                else
                    _states[id.Value] = (await _repository.GetStateAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToStateContract();
            }

            if (!dictionary.HasValue || dictionary.Value == LocationDictionary.Cities)
            {
                if (!id.HasValue)
                    _cities = new((await _repository.GetCitiesAsync()).Select(cityEntity => cityEntity.MapToCityContract()).ToDictionary(city => city.CityId));
                else
                    _cities[id.Value] = (await _repository.GetCityAsync(id.Value) ?? throw new BadRequestException($"{nameof(id)} is invalid")).MapToCityContract();
            }
        }

        #endregion

        #region Search

        public List<LocationCountryContract> SearchCountries(string keyword) =>
            ILocationMemoryCache.Countries
                .Where(country =>
                    country.Value.ThreeLetterCode.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) ||
                    country.Value.Name.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
                )
                .Take(_configuration.Limit.SearchCountriesLimit)
                .Select(country => country.Value)
                .ToList();

        public List<LocationCityContract> SearchCities(string keyword) =>
            ILocationMemoryCache.Cities
                .Where(city => city.Value.Name.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(city => city.Value.IsCapital)
                .Take(_configuration.Limit.SearchCitiesLimit)
                .Select(city => city.Value)
                .ToList();

        #endregion

        #region Get  

        public LocationCityContract GetCity((decimal latitude, decimal longitude) coordinates)
        {
            var point = new Point(new Coordinate((double)coordinates.latitude, (double)coordinates.longitude)) { SRID = _configuration.Default.SpatialReferenceId };
            var coordinateLookup = LocationExtension.GetCoordinatesLookup(coordinates);

            return ILocationMemoryCache.Cities
                .Where(city => city.Value.CoordinatesLookup == coordinateLookup)
                .OrderBy(city => new Point(new Coordinate((double)city.Value.Latitude, (double)city.Value.Longitude)) { SRID = _configuration.Default.SpatialReferenceId }.Distance(point))
                .First().Value;
        }

        #endregion
    }

    public interface ILocationMemoryCache
    {
        Task MemoryCacheRefreshAsync(LocationDictionary? dictionary = null, int? id = null);

        public static IReadOnlyDictionary<int, LocationCountryContract> Countries => LocationService._countries;
        public static IReadOnlyDictionary<int, LocationStateContract> States => LocationService._states;
        public static IReadOnlyDictionary<int, LocationCityContract> Cities => LocationService._cities;

        public static LocationCountryContract GetCountry(int countryId) =>
            Countries.TryGetValue(countryId, out LocationCountryContract? country)
                ? country
                : throw new BadRequestException($"{nameof(countryId)} is invalid");

        public static LocationStateContract GetState(int stateId) =>
            States.TryGetValue(stateId, out LocationStateContract? state)
                ? state
                : throw new BadRequestException($"{nameof(stateId)} is invalid");

        public static LocationCityContract GetCity(int cityId) =>
            Cities.TryGetValue(cityId, out LocationCityContract? city)
                ? city
                : throw new BadRequestException($"{nameof(cityId)} is invalid");
    }

    public interface ILocationService : ILocationMemoryCache
    {
        LocationCityContract GetCity((decimal latitude, decimal longitude) coordinates);
        List<LocationCityContract> SearchCities(string keyword);
        List<LocationCountryContract> SearchCountries(string keyword);
    }
}