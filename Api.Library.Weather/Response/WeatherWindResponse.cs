using RestSharp.Deserializers;

namespace Api.Library.Weather.Response
{
    internal class WeatherWindResponse
    {
        [DeserializeAs(Name = "speed")]
        public decimal Speed { get; set; }

        [DeserializeAs(Name = "deg")]
        public decimal Degrees { get; set; }
    }
}