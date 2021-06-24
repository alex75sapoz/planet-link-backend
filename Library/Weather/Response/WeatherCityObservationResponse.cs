using RestSharp.Deserializers;
using System;
using System.Collections.Generic;

namespace Library.Weather.Response
{
    class WeatherCityObservationResponse : WeatherErrorResponse
    {
        [DeserializeAs(Name = "id")]
        public int OpenWeatherId { get; set; }

        [DeserializeAs(Name = "name")]
        public string CityName { get; set; } = default!;

        //Data provider returns timezone offset relative to the requested city
        [DeserializeAs(Name = "timezone")]
        public int TimezoneOffset { get; set; }

        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "dt")]
        public DateTimeOffset CreatedOn { get; set; }

        [DeserializeAs(Name = "visibility")]
        public decimal Visibility { get; set; }

        [DeserializeAs(Name = "base")]
        public string Base { get; set; } = default!;

        [DeserializeAs(Name = "coord")]
        public WeatherCoordinateResponse Coordinates { get; set; } = default!;

        [DeserializeAs(Name = "weather")]
        public List<WeatherConditionResponse> Conditions { get; set; } = default!;

        [DeserializeAs(Name = "main")]
        public WeatherTemperatureResponse Temperature { get; set; } = default!;

        [DeserializeAs(Name = "wind")]
        public WeatherWindResponse Wind { get; set; } = default!;

        [DeserializeAs(Name = "clouds")]
        public WeatherCloudResponse Cloud { get; set; } = default!;

        [DeserializeAs(Name = "rain")]
        public WeatherRainResponse Rain { get; set; } = default!;

        [DeserializeAs(Name = "snow")]
        public WeatherSnowResponse Snow { get; set; } = default!;

        [DeserializeAs(Name = "sys")]
        public WeatherObservationCityResponse City { get; set; } = default!;
    }

    public class WeatherObservationCityResponse
    {
        [DeserializeAs(Name = "country")]
        public string Country { get; set; } = default!;

        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "sunrise")]
        public DateTimeOffset SunriseOn { get; set; }

        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "sunset")]
        public DateTimeOffset SunsetOn { get; set; }
    }
}