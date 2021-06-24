using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    class WeatherSnowResponse
    {
        [DeserializeAs(Name = "1h")]
        public decimal OneHourVolume { get; set; }

        [DeserializeAs(Name = "3h")]
        public decimal ThreeHourVolume { get; set; }
    }
}