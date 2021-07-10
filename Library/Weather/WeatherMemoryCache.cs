using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Weather
{
    public interface IWeatherMemoryCache
    {
        public static bool IsReady => WeatherMemoryCache.IsReady;

        public static IReadOnlyDictionary<int, WeatherEmotionContract> WeatherEmotions => WeatherMemoryCache.WeatherEmotions;
        public static IReadOnlyDictionary<int, WeatherCityUserEmotionContract> WeatherCityUserEmotions => WeatherMemoryCache.WeatherCityUserEmotions;
    }

    static class WeatherMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, WeatherEmotionContract> WeatherEmotions = new();
        public static readonly ConcurrentDictionary<int, WeatherCityUserEmotionContract> WeatherCityUserEmotions = new();

        public static async Task RefreshAsync(WeatherRepository repository)
        {
            var emotions = (await repository.GetEmotionsAsync()).Select(emotionEntity => emotionEntity.MapToEmotionContract()).ToDictionary(emotion => emotion.EmotionId);
            var cityUserEmotions = (await repository.GetCityUserEmotionsAsync(DateTimeOffset.Now.AddDays(-1))).Select(cityUserEmotionEntity => cityUserEmotionEntity.MapToCityUserEmotionContract()).ToDictionary(cityUserEmotion => cityUserEmotion.CityUserEmotionId);

            //Emotions
            foreach (var emotion in emotions)
                WeatherEmotions[emotion.Key] = emotion.Value;

            foreach (var emotion in WeatherEmotions.Where(emotion => !emotions.ContainsKey(emotion.Key)).ToList())
                WeatherEmotions.TryRemove(emotion);

            //CityUserEmotions
            foreach (var cityUserEmotion in cityUserEmotions)
                WeatherCityUserEmotions[cityUserEmotion.Key] = cityUserEmotion.Value;

            foreach (var cityUserEmotion in WeatherCityUserEmotions.Where(cityUserEmotion => !cityUserEmotions.ContainsKey(cityUserEmotion.Key)).ToList())
                WeatherCityUserEmotions.TryRemove(cityUserEmotion);

            IsReady = true;
        }
    }
}