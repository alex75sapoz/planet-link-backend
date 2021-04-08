namespace Library.StockMarket.Contract
{
    public class StockMarketUserAlertTypeCountContract
    {
        public int Count { get; internal set; }
        public decimal Points { get; internal set; }
        public StockMarketAlertTypeContract AlertType { get; internal set; }
    }
}