using System;

namespace Library.Weather.Contract
{
    public class WeatherCityForecastContract
    {
        public string Title { get; internal set; }
        public string LongTitle { get; internal set; }
        public string Icon { get; internal set; }
        public decimal Current { get; internal set; }
        public decimal FeelsLike { get; internal set; }
        public decimal Min { get; internal set; }
        public decimal Max { get; internal set; }
        public decimal WindSpeed { get; internal set; }
        public decimal WindDegrees { get; internal set; }
        public decimal Pressure { get; internal set; }
        public decimal Humidity { get; internal set; }
        public decimal Cloudiness { get; internal set; }
        public decimal Rain { get; internal set; }
        public decimal Snow { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
        public DateTimeOffset SunriseOn { get; internal set; }
        public DateTimeOffset SunsetOn { get; internal set; }
    }
}