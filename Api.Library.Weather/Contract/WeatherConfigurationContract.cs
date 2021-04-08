using System.Collections.Generic;

namespace Api.Library.Weather.Contract
{
    public class WeatherConfigurationContract
    {
        public List<WeatherEmotionContract> Emotions { get; internal set; }
    }
}