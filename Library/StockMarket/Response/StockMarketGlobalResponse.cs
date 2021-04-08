using RestSharp.Deserializers;

namespace Library.StockMarket.Response
{
    internal class StockMarketGlobalResponse
    {
        [DeserializeAs(Name = "isTheStockMarketOpen")]
        public bool IsStockMarketOpen { get; set; }

        [DeserializeAs(Name = "isTheEuronextMarketOpen")]
        public bool IsEuronextMarketOpen { get; set; }

        [DeserializeAs(Name = "isTheForexMarketOpen")]
        public bool IsForexMarketOpen { get; set; }

        [DeserializeAs(Name = "isTheCryptoMarketOpen")]
        public bool IsCryptoMarketOpen { get; set; }
    }
}