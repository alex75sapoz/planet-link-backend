using System.Text.Json.Serialization;

namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteContract
    {
        [JsonIgnore]
        public int ExchangeId { get; internal set; }

        public int QuoteId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Symbol { get; internal set; } = default!;

        public StockMarketExchangeContract Exchange => IStockMarketService.GetExchange(ExchangeId);
    }
}