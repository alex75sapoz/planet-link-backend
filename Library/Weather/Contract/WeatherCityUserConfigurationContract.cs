using System.Text.Json.Serialization;

namespace Library.Weather.Contract
{
    public class WeatherCityUserConfigurationContract
    {
        [JsonIgnore]
        public int? EmotionId { get; internal set; }

        public int SelectionsToday { get; internal set; }
        public int LimitToday { get; internal set; }

        public WeatherEmotionContract Emotion => EmotionId.HasValue ? IWeatherMemoryCache.WeatherEmotions[EmotionId.Value] : null;
    }
}