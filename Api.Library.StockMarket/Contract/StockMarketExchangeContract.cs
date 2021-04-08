using System.Text.Json.Serialization;

namespace Api.Library.StockMarket.Contract
{
    public class StockMarketExchangeContract
    {
        [JsonIgnore]
        public string FinancialModelingPrepId { get; internal set; }

        public int ExchangeId { get; internal set; }
        public string Name { get; internal set; }
    }
}