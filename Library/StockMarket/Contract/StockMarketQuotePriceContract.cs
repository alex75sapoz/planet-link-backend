using System;

namespace Library.StockMarket.Contract
{
    public class StockMarketQuotePriceContract
    {
        public decimal Current { get; internal set; }
        public decimal CurrentChange { get; internal set; }
        public decimal CurrentChangePercent { get; internal set; }
        public decimal Open { get; internal set; }
        public decimal OpenChange { get; internal set; }
        public decimal OpenChangePercent { get; internal set; }
        public decimal High { get; internal set; }
        public decimal HighChange { get; internal set; }
        public decimal HighChangePercent { get; internal set; }
        public decimal Low { get; internal set; }
        public decimal LowChange { get; internal set; }
        public decimal LowChangePercent { get; internal set; }
        public decimal PreviousClose { get; internal set; }
        public decimal? Volume { get; internal set; }
        public decimal? AverageVolume { get; internal set; }
        public decimal? FiftyDayMovingAverage { get; internal set; }
        public decimal? TwoHundredDayMovingAverage { get; internal set; }
        public decimal? OneYearHigh { get; internal set; }
        public decimal? OneYearLow { get; internal set; }
        public decimal? MarketCapitalization { get; internal set; }
        public decimal? SharesOutstanding { get; internal set; }
        public decimal? EarningsPerShare { get; internal set; }
        public decimal? PriceToEarningsRatio { get; internal set; }
        public DateTimeOffset? EarningsPerShareAnnouncedOn { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
    }
}