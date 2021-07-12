using System.Text.Json.Serialization;

namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteUserConfigurationContract
    {
        [JsonIgnore]
        public int? EmotionId { get; internal set; }

        public int SelectionsToday { get; internal set; }
        public int LimitToday { get; internal set; }

        public StockMarketEmotionContract? Emotion => EmotionId.HasValue ? IStockMarketMemoryCache.StockMarketEmotions[EmotionId.Value] : null;
    }
}