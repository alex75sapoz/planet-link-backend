using Api.Library.StockMarket.Contract;
using Api.Library.StockMarket.Entity;
using Api.Library.StockMarket.Response;
using Api.Library.User;
using System.Collections.Generic;

namespace Api.Library.StockMarket
{
    internal static class StockMarketMapper
    {
        public static StockMarketExchangeContract MaptToExchangeContract(this StockMarketExchangeEntity src) => new()
        {
            ExchangeId = src.ExchangeId,
            FinancialModelingPrepId = src.FinancialModelingPrepId,
            Name = src.Name
        };

        public static StockMarketTimeframeContract MaptToTimeframeContract(this StockMarketTimeframeEntity src) => new()
        {
            TimeframeId = src.TimeframeId,
            Name = src.Name,
            Prefix = src.Prefix,
            Suffix = src.Suffix,
            Multiplier = src.Multiplier
        };

        public static StockMarketAlertTypeContract MaptToAlertTypeContract(this StockMarketAlertTypeEntity src) => new()
        {
            AlertTypeId = src.AlertTypeId,
            Name = src.Name
        };

        public static StockMarketAlertCompletedTypeContract MaptToAlertCompletedTypeContract(this StockMarketAlertCompletedTypeEntity src) => new()
        {
            AlertCompletedTypeId = src.AlertCompletedTypeId,
            Name = src.Name
        };

        public static StockMarketQuoteContract MapToQuoteContract(this StockMarketQuoteEntity src) => new()
        {
            QuoteId = src.QuoteId,
            Symbol = src.Symbol,
            Name = src.Name,
            Exchange = StockMarketMemoryCache.StockMarketExchanges[src.ExchangeId]
        };

        public static StockMarketGlobalContract MapToGlobalContract(this StockMarketGlobalResponse src) => new()
        {
            IsStockMarketOpen = src.IsStockMarketOpen,
            IsEuronextMarketOpen = src.IsEuronextMarketOpen,
            IsForexMarketOpen = src.IsForexMarketOpen,
            IsCryptoMarketOpen = src.IsCryptoMarketOpen
        };

        public static StockMarketQuoteCompanyContract MapToQuoteCompanyContract(this StockMarketQuoteCompanyResponse src) => new()
        {
            Name = src.Name,
            Description = src.Description,
            Exchange = src.Exchange,
            Industry = src.Industry,
            WebsiteUrl = src.WebsiteUrl,
            ChiefExecutiveOfficer = src.ChiefExecutiveOfficer,
            Sector = src.Sector,
            Country = src.Country,
            Employees = src.Employees,
            Phone = src.Phone,
            Address = src.Address,
            City = src.City,
            State = src.State,
            Zipcode = src.Zipcode,
            LogoUrl = src.LogoUrl,
            Beta = src.Beta,
            InitialPublicOfferingOn = src.InitialPublicOfferingOn
        };

        public static StockMarketQuotePriceContract MapToQuotePriceContract(this StockMarketQuotePriceResponse src) => new()
        {
            Current = src.Current,
            CurrentChange = src.Current - src.PreviousClose,
            CurrentChangePercent = StockMarketExtension.GetChangePercent(from: src.PreviousClose, to: src.Current),
            PreviousClose = src.PreviousClose,
            Open = src.Open,
            OpenChange = src.Open - src.PreviousClose,
            OpenChangePercent = StockMarketExtension.GetChangePercent(from: src.PreviousClose, to: src.Open),
            High = src.High,
            HighChange = src.High - src.PreviousClose,
            HighChangePercent = StockMarketExtension.GetChangePercent(from: src.PreviousClose, to: src.High),
            Low = src.Low,
            LowChange = src.Low - src.PreviousClose,
            LowChangePercent = StockMarketExtension.GetChangePercent(from: src.PreviousClose, to: src.Low),
            Volume = src.Volume,
            AverageVolume = src.AverageVolume,
            FiftyDayMovingAverage = src.FiftyDayMovingAverage,
            TwoHundredDayMovingAverage = src.TwoHundredDayMovingAverage,
            OneYearHigh = src.OneYearHigh,
            OneYearLow = src.OneYearLow,
            MarketCapitalization = src.MarketCapitalization,
            SharesOutstanding = src.SharesOutstanding,
            EarningsPerShare = src.EarningsPerShare,
            PriceToEarningsRatio = src.PriceToEarningsRatio,
            EarningsPerShareAnnouncedOn = src.EarningsPerShareAnnouncedOn,
            CreatedOn = src.CreatedOn
        };

        public static StockMarketQuoteCandleContract MapToQuoteCandleContract(this StockMarketQuoteCandleResponse src, List<StockMarketQuoteUserAlertContract> quoteUserAlerts, int timeframeMultiplier) => new()
        {
            Open = src.Open,
            High = src.High,
            Low = src.Low,
            Close = src.Close,
            Volume = src.Volume,
            CreatedOn = src.CreatedOn,
            TimeframeMultiplier = timeframeMultiplier,
            QuoteUserAlerts = quoteUserAlerts
        };

        public static StockMarketQuoteUserAlertContract MapToQuoteUserAlertContract(this StockMarketQuoteUserAlertEntity src) => new()
        {
            QuoteUserAlertId = src.QuoteUserAlertId,
            Buy = src.Buy,
            Sell = src.Sell,
            StopLoss = src.StopLoss,
            CreatedOn = src.CreatedOn,
            CompletedSell = src.CompletedSell,
            CompletedOn = src.CompletedOn,
            Quote = StockMarketMemoryCache.StockMarketQuotes[src.QuoteId],
            User = IUserMemoryCache.Users[src.UserId],
            AlertType = StockMarketMemoryCache.StockMarketAlertTypes[src.AlertTypeId],
            AlertCompletedType = src.AlertCompletedTypeId.HasValue
                ? StockMarketMemoryCache.StockMarketAlertCompletedTypes[src.AlertCompletedTypeId.Value]
                : null
        };

        public static StockMarketQuoteUserEmotionContract MapToQuoteUserEmotionContract(this StockMarketQuoteUserEmotionEntity src) => new()
        {
            QuoteUserEmotionId = src.QuoteUserEmotionId,
            CreatedOn = src.CreatedOn,
            Quote = StockMarketMemoryCache.StockMarketQuotes[src.QuoteId],
            User = IUserMemoryCache.Users[src.UserId],
            Emotion = StockMarketMemoryCache.StockMarketEmotions[src.EmotionId]
        };

        public static StockMarketEmotionContract MapToEmotionContract(this StockMarketEmotionEntity src) => new()
        {
            EmotionId = src.EmotionId,
            Name = src.Name,
            Emoji = src.Emoji
        };

        public static StockMarketQuoteReverseSplitContract MapToReverseSplitContract(this StockMarketQuoteReverseSplitResponse src) => new()
        {
            Numerator = src.Numerator,
            Denominator = src.Denominator,
            CreatedOn = src.CreatedOn
        };
    }
}