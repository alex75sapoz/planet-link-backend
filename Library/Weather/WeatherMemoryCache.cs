using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Weather
{
    static class WeatherMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, WeatherEmotionContract> Emotions = new();
        public static readonly ConcurrentDictionary<int, WeatherCityUserEmotionContract> CityUserEmotions = new();

        public static async Task LoadAsync(WeatherRepository repository)
        {
            if (IsReady) return;

            var emotions = (await repository.GetEmotionsAsync()).Select(emotionEntity => emotionEntity.MapToEmotionContract()).ToList();
            var cityUserEmotions = (await repository.GetCityUserEmotionsAsync(DateTimeOffset.Now.AddDays(-1))).Select(cityUserEmotionEntity => cityUserEmotionEntity.MapToCityUserEmotionContract()).ToList();

            foreach (var emotion in emotions)
                Emotions[emotion.EmotionId] = emotion;

            foreach (var cityUserEmotion in cityUserEmotions)
                CityUserEmotions[cityUserEmotion.CityUserEmotionId] = cityUserEmotion;

            IsReady = true;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task TrimAsync(WeatherRepository repository)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            foreach (var cityUserEmotion in CityUserEmotions.Where(cityUserEmotion => cityUserEmotion.Value.CreatedOn < DateTimeOffset.Now.AddDays(-1)).ToList())
                CityUserEmotions.TryRemove(cityUserEmotion);

            await Task.CompletedTask;
        }
    }
}