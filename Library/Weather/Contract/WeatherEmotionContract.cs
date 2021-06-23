namespace Library.Weather.Contract
{
    public class WeatherEmotionContract
    {
        public WeatherEmotionContract()
        {
            Name = default!;
            Emoji = default!;
        }

        public int EmotionId { get; internal set; }
        public string Name { get; internal set; }
        public string Emoji { get; internal set; }
    }
}