using Library.Base;
using RestSharp.Deserializers;
using System;

namespace Library.StockMarket.Response
{
    internal class StockMarketQuoteCandleResponse
    {
        public decimal Open { get; set; }

        public decimal Low { get; set; }

        public decimal High { get; set; }

        public decimal Close { get; set; }

        public long? Volume { get; set; }

        //Data provider returns datetime string at eastern timezone without offset
        [DeserializeAs(Name = "date")]
        public DateTimeOffset CreatedOn { get => CreatedOnAtEasternTimezone; set => CreatedOnAtEasternTimezone = value.DateTime.SetTimezone(BaseExtension.EasternTimezone); }
        private DateTimeOffset CreatedOnAtEasternTimezone { get; set; }
    }
}