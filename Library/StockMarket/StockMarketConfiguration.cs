﻿namespace Library.StockMarket
{
    public class StockMarketConfiguration
    {
        public StockMarketLimit Limit { get; set; } = default!;
        public StockMarketDuration Duration { get; set; } = default!;
        public StockMarketRequirement Requirement { get; set; } = default!;
        public StockMarketFinancialModelingPrepApi FinancialModelingPrepApi { get; set; } = default!;

        public class StockMarketLimit
        {
            public int SearchQuotesLimit { get; set; }
            public int SearchQuoteUserAlertsLimit { get; set; }
            public int CreateQuoteUserAlertLimit { get; set; }
            public int CreateQuoteUserAlertSellPointsLimit { get; set; }
            public int CreateQuoteUserAlertStopLossPointsLimit { get; set; }
            public int CreateQuoteUserEmotionLimit { get; set; }
        }

        public class StockMarketDuration
        {
            public int GlobalCacheDurationInSeconds { get; set; }
            public int QuotePriceCacheDurationIsSeconds { get; set; }
            public int QuoteCompanyCacheDurationInSeconds { get; set; }
            public int QuoteCandlesCacheDurationInSeconds { get; set; }
            public int QuoteReverseSplitsCacheDurationInSeconds { get; set; }
        }

        public class StockMarketRequirement
        {
            public int CreateQuoteUserAlertMinimumFollowersCount { get; set; }
            public int CreateQuoteUserAlertMinimumFollowingsCount { get; set; }
            public int CreateQuoteUserAlertMinimumStocktwitsCreatedOnAgeInMonths { get; set; }
            public int CreateQuoteUserAlertMinimumPostsCount { get; set; }
            public int CreateQuoteUserAlertMinimumLikesCount { get; set; }
            public int CreateQuoteUserAlertMinimumWatchlistQuotesCount { get; set; }
        }

        public class StockMarketFinancialModelingPrepApi
        {
            public string Server { get; set; } = default!;
            public string AuthenticationKey { get; set; } = default!;
        }
    }
}