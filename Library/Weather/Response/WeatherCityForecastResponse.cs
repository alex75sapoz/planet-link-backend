using RestSharp.Deserializers;
using System;
using System.Collections.Generic;

namespace Library.Weather.Response
{
    internal class WeatherCityForecastResponse
    {
        public WeatherCityForecastResponse()
        {
            DatetimeText = default!;
            Temperature = default!;
            Conditions = default!;
            Cloud = default!;
            Wind = default!;
            Rain = default!;
            Snow = default!;
        }

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