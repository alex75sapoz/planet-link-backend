using System.Text.Json.Serialization;

namespace Library.Weather.Contract
{
    public class WeatherCityEmotionCountContract
    {
        [JsonIgnore]
        public int EmotionId { get; internal set; }

        public int CityCount { get; internal set; }
        public int GlobalCount { get; internal set; }

        public WeatherEmotionContract Emotion => IWeatherMemoryCache.Emotions[EmotionId];
    }
}