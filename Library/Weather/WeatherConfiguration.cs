namespace Library.Weather
{
    public class WeatherConfiguration
    {
        public WeatherLimit Limit { get; set; } = default!;
        public WeatherDuration Duration { get; set; } = default!;
        public WeatherOpenWeatherApi OpenWeatherApi { get; set; } = default!;

        public class WeatherLimit
        {
            public int CreateCityUserEmotionLimit { get; set; }
        }

        public class WeatherDuration
        {
            public int CityObservationCacheDurationInSeconds { get; set; }
            public int CityForecastsCacheDurationInSeconds { get; set; }
        }

        public class WeatherOpenWeatherApi
        {
            public string Server { get; set; } = default!;
            public string AuthenticationKey { get; set; } = default!;
        }
    }
}