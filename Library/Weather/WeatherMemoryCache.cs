using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Weather
{
    public interface IWeatherMemoryCache
    {
        public static IReadOnlyDictionary<int, WeatherEmotionContract> WeatherEmotions => WeatherMemoryCache.WeatherEmotions;
        public static IReadOnlyDictionary<int, WeatherCityUserEmotionContract> WeatherCityUserEmotions => WeatherMemoryCache.WeatherCityUserEmotions;
    }

    static class WeatherMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, WeatherEmotionContract> WeatherEmotions = new();
        public static readonly ConcurrentDictionary<int, WeatherCityUserEmotionContract> WeatherCityUserEmotions = new();

        public static async Task LoadAsync(WeatherRepository repository)
        {
            if (IsReady) return;

            var emotions = (await repository.GetEmotionsAsync()).Select(emotionEntity => emotionEntity.MapToEmotionContract()).ToList();
            var cityUserEmotions = (await repository.GetCityUserEmotionsAsync(DateTimeOffset.Now.AddDays(-1))).Select(cityUserEmotionEntity => cityUserEmotionEntity.MapToCityUserEmotionContract()).ToList();

            foreach (var emotion in emotions)
                WeatherEmotions[emotion.EmotionId] = emotion;

            foreach (var cityUserEmotion in cityUserEmotions)
                WeatherCityUserEmotions[cityUserEmotion.CityUserEmotionId] = cityUserEmotion;

            IsReady = true;
        }

        public static async Task TrimAsync(WeatherRepository repository)
        {
            foreach (var cityUserEmotion in WeatherCityUserEmotions.Where(cityUserEmotion => cityUserEmotion.Value.CreatedOn < DateTimeOffset.Now.AddDays(-1)).ToList())
                WeatherCityUserEmotions.TryRemove(cityUserEmotion);

            await Task.CompletedTask;
        }
    }
}