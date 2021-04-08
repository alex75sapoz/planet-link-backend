using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    internal class WeatherConditionResponse
    {
        [DeserializeAs(Name = "id")]
        public int WeatherConditionId { get; set; }

        [DeserializeAs(Name = "main")]
        public string Name { get; set; }

        [DeserializeAs(Name = "description")]
        public string Description { get; set; }

        [DeserializeAs(Name = "icon")]
        public string Icon { get; set; }
    }
}