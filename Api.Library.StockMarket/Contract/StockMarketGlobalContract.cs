namespace Api.Library.StockMarket.Contract
{
    public class StockMarketGlobalContract
    {
        public bool IsStockMarketOpen { get; internal set; }
        public bool IsEuronextMarketOpen { get; internal set; }
        public bool IsForexMarketOpen { get; internal set; }
        public bool IsCryptoMarketOpen { get; internal set; }
    }
}