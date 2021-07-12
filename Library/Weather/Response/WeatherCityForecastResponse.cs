using RestSharp.Deserializers;
using System;
using System.Collections.Generic;

namespace Library.Weather.Response
{
    class WeatherCityForecastResponse
    {
        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "dt")]
        public DateTimeOffset CreatedOn { get; set; }

        [DeserializeAs(Name = "dt_txt")]
        public string DatetimeText { get; set; } = default!;

        [DeserializeAs(Name = "main")]
        public WeatherTemperatureResponse Temperature { get; set; } = default!;

        [DeserializeAs(Name = "weather")]
        public List<WeatherConditionResponse> Conditions { get; set; } = default!;

        [DeserializeAs(Name = "clouds")]
        public WeatherCloudResponse Cloud { get; set; } = default!;

        [DeserializeAs(Name = "wind")]
        public WeatherWindResponse Wind { get; set; } = default!;

        [DeserializeAs(Name = "rain")]
        public WeatherRainResponse Rain { get; set; } = default!;

        [DeserializeAs(Name = "snow")]
        public WeatherSnowResponse Snow { get; set; } = default!;
    }
}