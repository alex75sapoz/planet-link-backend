using RestSharp.Deserializers;
using System;

namespace Library.StockMarket.Response
{
    internal class StockMarketQuotePriceResponse
    {
        public string Symbol { get; set; }

        public string Name { get; set; }

        public string Exchange { get; set; }

        [DeserializeAs(Name = "price")]
        public decimal Current { get; set; }

        [DeserializeAs(Name = "dayHigh")]
        public decimal High { get; set; }

        [DeserializeAs(Name = "dayLow")]
        public decimal Low { get; set; }

        public decimal Open { get; set; }

        public decimal PreviousClose { get; set; }

        public decimal? Volume { get; set; }

        [DeserializeAs(Name = "avgVolume")]
        public decimal? AverageVolume { get; set; }

        [DeserializeAs(Name = "priceAvg50")]
        public decimal? FiftyDayMovingAverage { get; set; }

        [DeserializeAs(Name = "priceAvg200")]
        public decimal? TwoHundredDayMovingAverage { get; set; }

        [DeserializeAs(Name = "yearHigh")]
        public decimal? OneYearHigh { get; set; }

        [DeserializeAs(Name = "yearLow")]
        public decimal? OneYearLow { get; set; }

        [DeserializeAs(Name = "marketCap")]
        public decimal? MarketCapitalization { get; set; }

        public decimal? SharesOutstanding { get; set; }

        [DeserializeAs(Name = "eps")]
        public decimal? EarningsPerShare { get; set; }

        [DeserializeAs(Name = "pe")]
        public decimal? PriceToEarningsRatio { get; set; }

        //Data provider returns UTC string value
        [DeserializeAs(Name = "earningsAnnouncement")]
        public DateTimeOffset? EarningsPerShareAnnouncedOn { get; set; }

        //Data provider returns UTC timestamp value
        [DeserializeAs(Name = "timestamp")]
        public DateTimeOffset CreatedOn { get; set; }

    }
}