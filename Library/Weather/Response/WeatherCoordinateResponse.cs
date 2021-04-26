﻿using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    internal class WeatherCoordinateResponse
    {
        [DeserializeAs(Name = "lat")]
        public decimal Latitude { get; set; }

        [DeserializeAs(Name = "lon")]
        public decimal Longitude { get; set; }
    }
}