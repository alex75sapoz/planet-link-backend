using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    class WeatherTemperatureResponse
    {
        [DeserializeAs(Name = "temp")]
        public decimal Current { get; set; }

        [DeserializeAs(Name = "feels_like")]
        public decimal FeelsLike { get; set; }

        [DeserializeAs(Name = "temp_min")]
        public decimal Min { get; set; }

        [DeserializeAs(Name = "temp_max")]
        public decimal Max { get; set; }

        [DeserializeAs(Name = "pressure")]
        public decimal Pressure { get; set; }

        [DeserializeAs(Name = "humidity")]
        public decimal Humidity { get; set; }
    }
}