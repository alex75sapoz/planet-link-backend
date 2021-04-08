namespace Api.Library.Weather.Contract
{
    public class WeatherCityUserConfigurationContract
    {
        public int SelectionsToday { get; internal set; }
        public int LimitToday { get; internal set; }

        public WeatherEmotionContract Emotion { get; internal set; }
    }
}