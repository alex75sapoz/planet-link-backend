using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    class WeatherCloudResponse
    {
        [DeserializeAs(Name = "all")]
        public decimal Cloudiness { get; set; }
    }
}