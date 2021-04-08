namespace Library.Weather.Contract
{
    public class WeatherCityEmotionCountContract
    {
        public int CityCount { get; internal set; }
        public int GlobalCount { get; internal set; }

        public WeatherEmotionContract Emotion { get; internal set; }
    }
}