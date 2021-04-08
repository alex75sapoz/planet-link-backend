using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    internal class WeatherErrorResponse
    {
        [DeserializeAs(Name = "cod")]
        public string Code { get; set; }

        [DeserializeAs(Name = "message")]
        public string Message { get; set; }
    }
}