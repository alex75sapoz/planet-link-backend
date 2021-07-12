using Library.User;
using Library.User.Contract;
using System;
using System.Text.Json.Serialization;

namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteUserAlertContract
    {
        [JsonIgnore]
        public int QuoteId { get; internal set; }
        [JsonIgnore]
        public int UserId { get; internal set; }
        [JsonIgnore]
        public int AlertTypeId { get; internal set; }
        [JsonIgnore]
        public int? AlertCompletedTypeId { get; internal set; }

        public int QuoteUserAlertId { get; internal set; }
        public decimal Buy { get; internal set; }
        public decimal Sell { get; internal set; }
        public decimal SellPoints => StockMarketExtension.GetChangePercent(from: Buy, to: Sell);
        public decimal StopLoss { get; internal set; }
        public decimal StopLossPoints => StockMarketExtension.GetChangePercent(from: Buy, to: StopLoss);
        public DateTimeOffset CreatedOn { get; internal set; }
        public decimal? CompletedSell { get; internal set; }
        public decimal? CompletedSellPoints => CompletedSell.HasValue ? StockMarketExtension.GetChangePercent(from: Buy, CompletedSell.Value) : CompletedSell;
        public DateTimeOffset? CompletedOn { get; internal set; }

        public StockMarketQuoteContract Quote => IStockMarketMemoryCache.StockMarketQuotes[QuoteId];
        public UserContract User => IUserMemoryCache.Users[UserId];
        public StockMarketAlertTypeContract Type => IStockMarketMemoryCache.StockMarketAlertTypes[AlertTypeId];
        public StockMarketAlertCompletedTypeContract? CompletedType => AlertCompletedTypeId.HasValue ? IStockMarketMemoryCache.StockMarketAlertCompletedTypes[AlertCompletedTypeId.Value] : null;
    }
}