namespace Api.Library.Weather.Contract
{
    public class WeatherEmotionContract
    {
        public int EmotionId { get; internal set; }
        public string Name { get; internal set; }
        public string Emoji { get; internal set; }
    }
}