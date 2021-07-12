using System.Text.Json.Serialization;

namespace Library.StockMarket.Contract
{
    public class StockMarketExchangeContract
    {
        [JsonIgnore]
        public string FinancialModelingPrepId { get; internal set; } = default!;

        public int ExchangeId { get; internal set; }
        public string Name { get; internal set; } = default!;
    }
}