using Api.Library.Location.Contract;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Api.Library.Location
{
    public interface ILocationMemoryCache
    {
        public static IReadOnlyDictionary<int, LocationCountryContract> LocationCountries => LocationMemoryCache.LocationCountries;
        public static IReadOnlyDictionary<int, LocationStateContract> LocationStates => LocationMemoryCache.LocationStates;
        public static IReadOnlyDictionary<int, LocationCityContract> LocationCities => LocationMemoryCache.LocationCities;
    }

    internal static class LocationMemoryCache
    {
        public static readonly ConcurrentDictionary<int, LocationCountryContract> LocationCountries = new();
        public static readonly ConcurrentDictionary<int, LocationStateContract> LocationStates = new();
        public static readonly ConcurrentDictionary<int, LocationCityContract> LocationCities = new();
    }
}