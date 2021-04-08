using Api.Library.Location;
using Api.Library.User;
using Api.Library.Weather.Contract;
using Api.Library.Weather.Entity;
using Api.Library.Weather.Response;
using System.Collections.Generic;
using System.Linq;

namespace Api.Library.Weather
{
    internal static class WeatherMapper
    {
        public static WeatherCityObservationContract MapToCityObservationContract(this WeatherCityObservationResponse src) => new()
        {
            Title = src.Conditions.First().Name,
            LongTitle = src.Conditions.GetLongTitle(),
            Icon = src.Conditions.First().Icon,
            Current = src.Temperature.Current,
            FeelsLike = src.Temperature.FeelsLike,
            Min = src.Temperature.Min,
            Max = src.Temperature.Max,
            WindSpeed = src.Wind.Speed,
            WindDegrees = src.Wind.Degrees,
            Pressure = src.Temperature.Pressure,
            Humidity = src.Temperature.Humidity,
            Cloudiness = src.Cloud.Cloudiness,
            Rain = src.Rain.OneHourVolume != 0
                ? src.Rain.OneHourVolume
                : src.Rain.ThreeHourVolume,
            Snow = src.Snow.OneHourVolume != 0
                ? src.Snow.OneHourVolume
                : src.Snow.ThreeHourVolume,
            CreatedOn = src.CreatedOn,
            SunriseOn = src.City.SunriseOn,
            SunsetOn = src.City.SunsetOn
        };

        public static List<WeatherCityForecastContract> MapToCityForecastContracts(this WeatherForecastResponse data) =>
            data.Forecasts.Select(src => new WeatherCityForecastContract()
            {
                Title = src.Conditions.First().Name,
                LongTitle = src.Conditions.GetLongTitle(),
                Icon = src.Conditions.First().Icon,
                Current = src.Temperature.Current,
                FeelsLike = src.Temperature.FeelsLike,
                Min = src.Temperature.Min,
                Max = src.Temperature.Max,
                WindSpeed = src.Wind.Speed,
                WindDegrees = src.Wind.Degrees,
                Pressure = src.Temperature.Pressure,
                Humidity = src.Temperature.Humidity,
                Cloudiness = src.Cloud.Cloudiness,
                Rain = src.Rain.OneHourVolume != 0
                    ? src.Rain.OneHourVolume
                    : src.Rain.ThreeHourVolume,
                Snow = src.Snow.OneHourVolume != 0
                    ? src.Snow.OneHourVolume
                    : src.Snow.ThreeHourVolume,
                CreatedOn = src.CreatedOn,
                SunriseOn = data.City.SunriseOn,
                SunsetOn = data.City.SunsetOn,
            }).ToList();

        public static WeatherCityUserEmotionContract MapToCityUserEmotionContract(this WeatherCityUserEmotionEntity src) => new()
        {
            CityUserEmotionId = src.CityUserEmotionId,
            CreatedOn = src.CreatedOn,
            City = ILocationMemoryCache.LocationCities[src.CityId],
            User = IUserMemoryCache.Users[src.UserId],
            Emotion = WeatherMemoryCache.WeatherEmotions[src.EmotionId]
        };

        public static WeatherEmotionContract MapToEmotionContract(this WeatherEmotionEntity src) => new()
        {
            EmotionId = src.EmotionId,
            Name = src.Name,
            Emoji = src.Emoji
        };
    }
}