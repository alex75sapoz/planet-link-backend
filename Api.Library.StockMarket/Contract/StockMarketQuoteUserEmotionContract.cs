using Api.Library.User.Contract;
using System;

namespace Api.Library.StockMarket.Contract
{
    public class StockMarketQuoteUserEmotionContract
    {
        public int QuoteUserEmotionId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public StockMarketQuoteContract Quote { get; internal set; }
        public UserContract User { get; internal set; }
        public StockMarketEmotionContract Emotion { get; internal set; }
    }
}