namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteEmotionCountContract
    {
        public int QuoteCount { get; internal set; }
        public int GlobalCount { get; internal set; }

        public StockMarketEmotionContract Emotion { get; internal set; }
    }
}