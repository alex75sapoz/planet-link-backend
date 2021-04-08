using Library.Weather.Contract;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Library.Weather
{
    public interface IWeatherMemoryCache
    {
        public static IReadOnlyDictionary<int, WeatherEmotionContract> WeatherEmotions => WeatherMemoryCache.WeatherEmotions;
        public static IReadOnlyDictionary<int, WeatherCityUserEmotionContract> WeatherCityUserEmotions => WeatherMemoryCache.WeatherCityUserEmotions;
    }

    internal static class WeatherMemoryCache
    {
        public static readonly ConcurrentDictionary<int, WeatherEmotionContract> WeatherEmotions = new();
        public static readonly ConcurrentDictionary<int, WeatherCityUserEmotionContract> WeatherCityUserEmotions = new();
    }
}