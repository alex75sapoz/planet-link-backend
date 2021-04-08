using Api.Library.User.Contract;
using System;

namespace Api.Library.StockMarket.Contract
{
    public class StockMarketQuoteUserAlertContract
    {
        public int QuoteUserAlertId { get; internal set; }
        public decimal Buy { get; internal set; }
        public decimal Sell { get; internal set; }
        public decimal SellPoints => StockMarketExtension.GetChangePercent(from: Buy, to: Sell);
        public decimal StopLoss { get; internal set; }
        public decimal StopLossPoints => StockMarketExtension.GetChangePercent(from: Buy, to: StopLoss);
        public DateTimeOffset CreatedOn { get; internal set; }
        public decimal? CompletedSell { get; internal set; }
        public decimal? CompletedSellPoints => CompletedSell.HasValue
            ? StockMarketExtension.GetChangePercent(from: Buy, CompletedSell.Value)
            : CompletedSell;
        public DateTimeOffset? CompletedOn { get; internal set; }

        public StockMarketQuoteContract Quote { get; internal set; }
        public UserContract User { get; internal set; }
        public StockMarketAlertTypeContract AlertType { get; internal set; }
        public StockMarketAlertCompletedTypeContract AlertCompletedType { get; internal set; }
    }
}