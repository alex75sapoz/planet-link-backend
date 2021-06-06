using System.Text.Json.Serialization;

namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteEmotionCountContract
    {
        [JsonIgnore]
        public int EmotionId { get; internal set; }

        public int QuoteCount { get; internal set; }
        public int GlobalCount { get; internal set; }

        public StockMarketEmotionContract Emotion => IStockMarketMemoryCache.StockMarketEmotions[EmotionId];
    }
}