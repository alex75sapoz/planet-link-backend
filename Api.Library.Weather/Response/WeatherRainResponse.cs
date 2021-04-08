﻿using RestSharp.Deserializers;

namespace Api.Library.Weather.Response
{
    internal class WeatherRainResponse
    {
        [DeserializeAs(Name = "1h")]
        public decimal OneHourVolume { get; set; }

        [DeserializeAs(Name = "3h")]
        public decimal ThreeHourVolume { get; set; }
    }
}