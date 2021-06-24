using RestSharp.Deserializers;
using System.Collections.Generic;

namespace Library.StockMarket.Response
{
    class StockMarketQuoteReverseSplitFullResponse
    {
        public string Symbol { get; set; } = default!;

        [DeserializeAs(Name = "historical")]
        public List<StockMarketQuoteReverseSplitResponse> ReverseSplits { get; set; } = default!;
    }
}
