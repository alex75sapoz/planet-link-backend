using Api.Library.Weather.Contract;
using Api.Library.Weather.Response;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Library.Weather
{
    internal static class WeatherExtension
    {
        public static string GetLongTitle(this List<WeatherConditionResponse> input) =>
            string.Join(' ', input.First().Description
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Select(word => $"{word[0].ToString().ToUpper()}{word[1..]}")
            );

        public static List<WeatherCityUserEmotionContract> GetCityUserEmotionsAtTimezoneToday(this IReadOnlyDictionary<int, WeatherCityUserEmotionContract> cityUserEmotions, DateTimeZone timezone, int? userId = null)
        {
            var datetimeOffsetAtTimezone = DateTimeOffset.Now.AtTimezone(timezone);

            return cityUserEmotions
                .Where(cityUserEmotion =>
                    (userId is null || cityUserEmotion.Value.User.UserId == userId) &&
                    cityUserEmotion.Value.CreatedOn.AtTimezone(timezone).Date == datetimeOffsetAtTimezone.Date
                )
                .Select(cityUserEmotion => cityUserEmotion.Value)
                .ToList();
        }
    }
}