using RestSharp.Deserializers;

namespace Library.Weather.Response
{
    class WeatherConditionResponse
    {
        [DeserializeAs(Name = "id")]
        public int WeatherConditionId { get; set; }

        [DeserializeAs(Name = "main")]
        public string Name { get; set; } = default!;

        [DeserializeAs(Name = "description")]
        public string Description { get; set; } = default!;

        [DeserializeAs(Name = "icon")]
        public string Icon { get; set; } = default!;
    }
}