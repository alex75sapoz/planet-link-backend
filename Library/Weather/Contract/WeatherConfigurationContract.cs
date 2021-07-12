using System.Collections.Generic;

namespace Library.Weather.Contract
{
    public class WeatherConfigurationContract
    {
        public List<WeatherEmotionContract> Emotions { get; internal set; } = default!;
    }
}