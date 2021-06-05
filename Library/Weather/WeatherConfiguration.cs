namespace Library.Weather
{
    public class WeatherConfiguration
    {
        public WeatherLimit Limit { get; set; }
        public WeatherDuration Duration { get; set; }
        public WeatherOpenWeatherApi OpenWeatherApi { get; set; }

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
            public string Server { get; set; }
            public string AuthenticationKey { get; set; }
        }
    }
}