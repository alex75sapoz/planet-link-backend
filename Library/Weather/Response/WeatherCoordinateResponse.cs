using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    class WeatherCoordinateResponse
    {
        [DeserializeAs(Name = "lat")]
        public decimal Latitude { get; set; }

        [DeserializeAs(Name = "lon")]
        public decimal Longitude { get; set; }
    }
}