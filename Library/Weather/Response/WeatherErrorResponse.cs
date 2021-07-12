using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    class WeatherErrorResponse
    {
        [DeserializeAs(Name = "cod")]
        public string Code { get; set; } = default!;

        [DeserializeAs(Name = "message")]
        public string Message { get; set; } = default!;
    }
}