namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteUserAlertRequirementConfigurationContract
    {
        public int MinimumFollowersCount { get; internal set; }
        public int MinimumFollowingsCount { get; internal set; }
        public int MinimumStocktwitsCreatedOnAgeInMonths { get; internal set; }
        public int MinimumPostsCount { get; internal set; }
        public int MinimumLikesCount { get; internal set; }
        public int MinimumWatchlistQuotesCount { get; internal set; }
        public int MaximumAlertsInProgressCount { get; internal set; }
    }
}