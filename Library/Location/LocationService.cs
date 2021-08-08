using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Library.Location
{
    public interface ILocationService
    {
        LocationCityContract GetCity((decimal latitude, decimal longitude) coordinates);
        List<LocationCityContract> SearchCities(string keyword);
        List<LocationCountryContract> SearchCountries(string keyword);

        #region Memory Cache

        internal static IReadOnlyDictionary<int, LocationCountryContract> Countries => LocationMemoryCache.Countries;
        internal static IReadOnlyDictionary<int, LocationStateContract> States => LocationMemoryCache.States;
        internal static IReadOnlyDictionary<int, LocationCityContract> Cities => LocationMemoryCache.Cities;

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

        #endregion
    }

    class LocationService : BaseService<LocationConfiguration, LocationRepository>, ILocationService
    {
        public LocationService(LocationConfiguration configuration, LocationRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region Search

        public List<LocationCountryContract> SearchCountries(string keyword) =>
            ILocationService.Countries
                .Where(country =>
                    country.Value.ThreeLetterCode.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) ||
                    country.Value.Name.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
                )
                .Take(_configuration.Limit.SearchCountriesLimit)
                .Select(country => country.Value)
                .ToList();

        public List<LocationCityContract> SearchCities(string keyword) =>
            ILocationService.Cities
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

            return ILocationService.Cities
                .Where(city => city.Value.CoordinatesLookup == coordinateLookup)
                .OrderBy(city => new Point(new Coordinate((double)city.Value.Latitude, (double)city.Value.Longitude)) { SRID = _configuration.Default.SpatialReferenceId }.Distance(point))
                .First().Value;
        }

        #endregion
    }
}