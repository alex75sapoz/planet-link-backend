using RestSharp.Deserializers;
using System.Collections.Generic;

namespace Library.StockMarket.Response
{
    class StockMarketQuoteCandleFullResponse
    {
        public string Symbol { get; set; } = default!;

        [DeserializeAs(Name = "historical")]
        public List<StockMarketQuoteCandleResponse> Candles { get; set; } = default!;
    }
}