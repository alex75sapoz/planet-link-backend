using RestSharp.Deserializers;
using System;
using System.Collections.Generic;

namespace Api.Library.Weather.Response
{
    internal class WeatherCityForecastResponse
    {
        //Data provider returns UTC timestamp
        [DeserializeAs(Name = "dt")]
        public DateTimeOffset CreatedOn { get; set; }

        [DeserializeAs(Name = "dt_txt")]
        public string DatetimeText { get; set; }

        [DeserializeAs(Name = "main")]
        public WeatherTemperatureResponse Temperature { get; set; }

        [DeserializeAs(Name = "weather")]
        public List<WeatherConditionResponse> Conditions { get; set; }

        [DeserializeAs(Name = "clouds")]
        public WeatherCloudResponse Cloud { get; set; }

        [DeserializeAs(Name = "wind")]
        public WeatherWindResponse Wind { get; set; }

        [DeserializeAs(Name = "rain")]
        public WeatherRainResponse Rain { get; set; }

        [DeserializeAs(Name = "snow")]
        public WeatherSnowResponse Snow { get; set; }
    }
}