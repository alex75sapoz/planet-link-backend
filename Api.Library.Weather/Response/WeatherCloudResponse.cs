﻿using RestSharp.Deserializers;

namespace Api.Library.Weather.Response
{
    internal class WeatherCloudResponse
    {
        [DeserializeAs(Name = "all")]
        public decimal Cloudiness { get; set; }
    }
}