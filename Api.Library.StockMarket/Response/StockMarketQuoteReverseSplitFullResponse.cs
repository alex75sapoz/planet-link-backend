using RestSharp.Deserializers;
using System.Collections.Generic;

namespace Api.Library.StockMarket.Response
{
    internal class StockMarketQuoteReverseSplitFullResponse
    {
        public string Symbol { get; set; }

        [DeserializeAs(Name = "historical")]
        public List<StockMarketQuoteReverseSplitResponse> ReverseSplits { get; set; }
    }
}
