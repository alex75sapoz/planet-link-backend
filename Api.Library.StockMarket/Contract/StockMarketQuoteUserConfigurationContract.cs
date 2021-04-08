namespace Api.Library.StockMarket.Contract
{
    public class StockMarketQuoteUserConfigurationContract
    {
        public int SelectionsToday { get; internal set; }
        public int LimitToday { get; internal set; }

        public StockMarketEmotionContract Emotion { get; internal set; }
    }
}