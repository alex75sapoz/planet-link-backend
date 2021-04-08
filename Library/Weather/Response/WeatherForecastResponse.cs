using RestSharp.Deserializers;
using System;
using System.Collections.Generic;

namespace Library.Weather.Response
{
    internal class WeatherForecastResponse : WeatherErrorResponse
    {
        [DeserializeAs(Name = "cnt")]
        public int TotalForecasts { get; set; }

        [DeserializeAs(Name = "city")]
        public WeatherForecastCityResponse City { get; set; }

        [DeserializeAs(Name = "list")]
        public List<WeatherCityForecastResponse> Forecasts { get; set; }
    }

    internal class WeatherForecastCityResponse
    {
        [DeserializeAs(Name = "id")]
        public int OpenWeatherId { get; set; }

        [DeserializeAs(Name = "name")]
        public string Name { get; set; }

        [DeserializeAs(Name = "coord")]
        public WeatherCoordinateResponse Coordinates { get; set; }

        [DeserializeAs(Name = "country")]
        public string Country { get; set; }

        //Data provider returns timezone offset relative to the requested city
        [DeserializeAs(Name = "timezone")]
        public int TimezoneOffset { get; set; }

        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "sunrise")]
        public DateTimeOffset SunriseOn { get; set; }

        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "sunset")]
        public DateTimeOffset SunsetOn { get; set; }
    }
}