using Library.User;
using Library.User.Contract;
using System;
using System.Text.Json.Serialization;

namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteUserEmotionContract
    {
        [JsonIgnore]
        public int QuoteId { get; internal set; }
        [JsonIgnore]
        public int UserId { get; internal set; }
        [JsonIgnore]
        public int EmotionId { get; internal set; }

        public int QuoteUserEmotionId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public StockMarketQuoteContract Quote => IStockMarketMemoryCache.StockMarketQuotes[QuoteId];
        public UserContract User => IUserMemoryCache.Users[UserId];
        public StockMarketEmotionContract Emotion => IStockMarketMemoryCache.StockMarketEmotions[EmotionId];
    }
}