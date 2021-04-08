using RestSharp.Deserializers;
using System;

namespace Api.Library.StockMarket.Response
{
    internal class StockMarketQuoteReverseSplitResponse
    {
        public decimal Numerator { get; set; }
        public decimal Denominator { get; set; }

        //Data provider returns date string at eastern timezone without offset
        [DeserializeAs(Name = "date")]
        public DateTimeOffset CreatedOn { get => CreatedOnAtEasternTimezone; set => CreatedOnAtEasternTimezone = value.DateTime.SetTimezone(LibraryExtension.EasternTimezone); }
        private DateTimeOffset CreatedOnAtEasternTimezone { get; set; }
    }
}