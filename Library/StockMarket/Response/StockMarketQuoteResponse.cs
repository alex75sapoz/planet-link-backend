using RestSharp.Deserializers;

namespace Library.StockMarket.Response
{
    class StockMarketQuoteResponse
    {
        public string Symbol { get; set; } = default!;

        public string Name { get; set; } = default!;

        [DeserializeAs(Name = "exchangeShortName")]
        public string Exchange { get; set; } = default!;
    }
}