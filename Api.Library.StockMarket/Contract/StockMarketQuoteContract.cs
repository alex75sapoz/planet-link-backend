namespace Api.Library.StockMarket.Contract
{
    public class StockMarketQuoteContract
    {
        public int QuoteId { get; internal set; }
        public string Name { get; internal set; }
        public string Symbol { get; internal set; }

        public StockMarketExchangeContract Exchange { get; internal set; }
    }
}