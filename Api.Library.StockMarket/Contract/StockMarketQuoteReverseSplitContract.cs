using System;

namespace Api.Library.StockMarket.Contract
{
    public class StockMarketQuoteReverseSplitContract
    {
        public decimal Numerator { get; internal set; }
        public decimal Denominator { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
    }
}