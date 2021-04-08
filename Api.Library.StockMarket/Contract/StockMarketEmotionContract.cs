namespace Api.Library.StockMarket.Contract
{
    public class StockMarketEmotionContract
    {
        public int EmotionId { get; internal set; }
        public string Name { get; internal set; }
        public string Emoji { get; internal set; }
    }
}