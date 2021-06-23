using RestSharp.Deserializers;
using System;
using System.Collections.Generic;

namespace Library.Weather.Response
{
    internal class WeatherCityObservationResponse : WeatherErrorResponse
    {
        public WeatherCityObservationResponse()
        {
            CityName = default!;
            Base = default!;
            Coordinates = default!;
            Conditions = default!;
            Temperature = default!;
            Wind = default!;
            Cloud = default!;
            Rain = default!;
            Snow = default!;
            City = default!;
        }

        [DeserializeAs(Name = "id")]
        public int OpenWeatherId { get; set; }

        [DeserializeAs(Name = "name")]
        public string CityName { get; set; }

        //Data provider returns timezone offset relative to the requested city
        [DeserializeAs(Name = "timezone")]
        public int TimezoneOffset { get; set; }

        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "dt")]
        public DateTimeOffset CreatedOn { get; set; }

        [DeserializeAs(Name = "visibility")]
        public decimal Visibility { get; set; }

        [DeserializeAs(Name = "base")]
        public string Base { get; set; }

        [DeserializeAs(Name = "coord")]
        public WeatherCoordinateResponse Coordinates { get; set; }

        [DeserializeAs(Name = "weather")]
        public List<WeatherConditionResponse> Conditions { get; set; }

        [DeserializeAs(Name = "main")]
        public WeatherTemperatureResponse Temperature { get; set; }

        [DeserializeAs(Name = "wind")]
        public WeatherWindResponse Wind { get; set; }

        [DeserializeAs(Name = "clouds")]
        public WeatherCloudResponse Cloud { get; set; }

        [DeserializeAs(Name = "rain")]
        public WeatherRainResponse Rain { get; set; }

        [DeserializeAs(Name = "snow")]
        public WeatherSnowResponse Snow { get; set; }

        [DeserializeAs(Name = "sys")]
        public WeatherObservationCityResponse City { get; set; }
    }

    public class WeatherObservationCityResponse
    {
        public WeatherObservationCityResponse()
        {
            Country = default!;
        }

        [DeserializeAs(Name = "country")]
        public string Country { get; set; }

        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "sunrise")]
        public DateTimeOffset SunriseOn { get; set; }

        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "sunset")]
        public DateTimeOffset SunsetOn { get; set; }
    }
}