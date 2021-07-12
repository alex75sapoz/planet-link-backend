namespace Library.Weather.Contract
{
    public class WeatherEmotionContract
    {
        public int EmotionId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Emoji { get; internal set; } = default!;
    }
}