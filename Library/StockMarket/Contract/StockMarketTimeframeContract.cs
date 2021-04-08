namespace Library.StockMarket.Contract
{
    public class StockMarketTimeframeContract
    {
        public int TimeframeId { get; internal set; }
        public string Name { get; internal set; }
        public int Prefix { get; internal set; }
        public string Suffix { get; internal set; }
        public int Multiplier { get; internal set; }
    }
}