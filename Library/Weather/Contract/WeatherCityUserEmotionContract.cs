using Library.Location;
using Library.Location.Contract;
using Library.User;
using Library.User.Contract;
using System;
using System.Text.Json.Serialization;

namespace Library.Weather.Contract
{
    public class WeatherCityUserEmotionContract
    {
        [JsonIgnore]
        public int CityId { get; internal set; }
        [JsonIgnore]
        public int UserId { get; internal set; }
        [JsonIgnore]
        public int EmotionId { get; internal set; }

        public int CityUserEmotionId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public LocationCityContract City => ILocationMemoryCache.LocationCities[CityId];
        public UserContract User => IUserMemoryCache.Users[UserId];
        public WeatherEmotionContract Emotion => IWeatherMemoryCache.WeatherEmotions[EmotionId];
    }
}