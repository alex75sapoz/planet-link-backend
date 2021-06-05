using System.Text.Json.Serialization;

namespace Library.StockMarket.Contract
{
    public class StockMarketUserAlertTypeCountContract
    {
        [JsonIgnore]
        public int AlertTypeId { get; internal set; }

        public int Count { get; internal set; }
        public decimal Points { get; internal set; }
        public StockMarketAlertTypeContract AlertType => IStockMarketMemoryCache.StockMarketAlertTypes[AlertTypeId];
    }
}