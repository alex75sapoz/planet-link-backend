using System.Collections.Generic;

namespace Library.Weather.Contract
{
    public class WeatherConfigurationContract
    {
        public WeatherConfigurationContract()
        {
            Emotions = new List<WeatherEmotionContract>();
        }

        public List<WeatherEmotionContract> Emotions { get; internal set; }
    }
}