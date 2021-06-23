using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    internal class WeatherErrorResponse
    {
        public WeatherErrorResponse()
        {
            Code = default!;
            Message = default!;
        }

        [DeserializeAs(Name = "cod")]
        public string Code { get; set; }

        [DeserializeAs(Name = "message")]
        public string Message { get; set; }
    }
}