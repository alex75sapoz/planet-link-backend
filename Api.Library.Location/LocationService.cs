using Api.Library.Location.Contract;
using Microsoft.Extensions.Caching.Memory;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Library.Location
{
    public interface ILocationService
    {
        LocationCityContract GetCity((decimal latitude, decimal longitude) coordinates);
        LocationCityContract GetCity(int cityId);
        LocationCountryContract GetCountry(int countryId);
        List<LocationCityContract> SearchCities(string keyword);
        List<LocationCountryContract> SearchCountries(string keyword);
    }

    internal class LocationService : LibraryService<LocationConfiguration, LocationRepository>, ILocationService
    {
        public LocationService(LocationConfiguration configuration, LocationRepository repository, IMemoryCache memoryCache) : base(configuration, repository, memoryCache) { }

        #region Search

        public List<LocationCountryContract> SearchCountries(string keyword) =>
            LocationMemoryCache.LocationCountries
                .Where(country =>
                    country.Value.ThreeLetterCode.StartsWith(keyword, StringComparison.OrdinalIgnoreCase) ||
                    country.Value.Name.StartsWith(keyword, StringComparison.OrdinalIgnoreCase)
                )
                .Take(_configuration.Limit.SearchCountriesLimit)
                .Select(country => country.Value)
                .ToList();

        public List<LocationCityContract> SearchCities(string keyword) =>
            LocationMemoryCache.LocationCities
                .Where(city => city.Value.Name.StartsWith(keyword, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(city => city.Value.IsCapital)
                .Take(_configuration.Limit.SearchCitiesLimit)
                .Select(city => city.Value)
                .ToList();

        #endregion

        #region Get

        public LocationCountryContract GetCountry(int countryId) =>
            LocationMemoryCache.LocationCountries.TryGetValue(countryId, out LocationCountryContract country)
                ? country
                : throw new BadRequestException($"{nameof(countryId)} is invalid");

        public LocationCityContract GetCity(int cityId) =>
            LocationMemoryCache.LocationCities.TryGetValue(cityId, out LocationCityContract city)
                ? city
                : throw new BadRequestException($"{nameof(cityId)} is invalid");

        public LocationCityContract GetCity((decimal latitude, decimal longitude) coordinates)
        {
            var point = new Point(new Coordinate((double)coordinates.latitude, (double)coordinates.longitude)) { SRID = _configuration.Default.SpatialReferenceId };
            var coordinateLookup = LocationExtension.GetCoordinatesLookup(coordinates);

            return LocationMemoryCache.LocationCities
                .Where(city => city.Value.CoordinatesLookup == coordinateLookup)
                .OrderBy(city => new Point(new Coordinate((double)city.Value.Latitude, (double)city.Value.Longitude)) { SRID = _configuration.Default.SpatialReferenceId }.Distance(point))
                .First().Value;
        }

        #endregion
    }
}