using RestSharp.Deserializers;
using System;

namespace Library.StockMarket.Response
{
    class StockMarketQuoteReverseSplitResponse
    {
        public decimal Numerator { get; set; }
        public decimal Denominator { get; set; }

        //Data provider returns date string at eastern timezone without offset
        [DeserializeAs(Name = "date")]
        public DateTimeOffset CreatedOn { get => CreatedOnAtEasternTimezone; set => CreatedOnAtEasternTimezone = value.DateTime.SetTimezone(BaseExtension.EasternTimezone); }
        private DateTimeOffset CreatedOnAtEasternTimezone { get; set; }
    }
}