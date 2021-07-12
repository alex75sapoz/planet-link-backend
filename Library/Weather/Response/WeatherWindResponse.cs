using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    class WeatherWindResponse
    {
        [DeserializeAs(Name = "speed")]
        public decimal Speed { get; set; }

        [DeserializeAs(Name = "deg")]
        public decimal Degrees { get; set; }
    }
}