using Api.Library.Location.Contract;
using Api.Library.User.Contract;
using System;

namespace Api.Library.Weather.Contract
{
    public class WeatherCityUserEmotionContract
    {
        public int CityUserEmotionId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public LocationCityContract City { get; internal set; }
        public UserContract User { get; internal set; }
        public WeatherEmotionContract Emotion { get; internal set; }
    }
}