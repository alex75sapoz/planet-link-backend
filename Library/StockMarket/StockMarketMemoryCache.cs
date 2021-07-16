using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.StockMarket
{
    public interface IStockMarketMemoryCache
    {
        public static IReadOnlyDictionary<int, StockMarketExchangeContract> StockMarketExchanges => StockMarketMemoryCache.StockMarketExchanges;
        public static IReadOnlyDictionary<int, StockMarketTimeframeContract> StockMarketTimeframes => StockMarketMemoryCache.StockMarketTimeframes;
        public static IReadOnlyDictionary<int, StockMarketAlertTypeContract> StockMarketAlertTypes => StockMarketMemoryCache.StockMarketAlertTypes;
        public static IReadOnlyDictionary<int, StockMarketAlertCompletedTypeContract> StockMarketAlertCompletedTypes => StockMarketMemoryCache.StockMarketAlertCompletedTypes;
        public static IReadOnlyDictionary<int, StockMarketEmotionContract> StockMarketEmotions => StockMarketMemoryCache.StockMarketEmotions;
        public static IReadOnlyDictionary<int, StockMarketQuoteContract> StockMarketQuotes => StockMarketMemoryCache.StockMarketQuotes;
        public static IReadOnlyDictionary<int, StockMarketQuoteUserAlertContract> StockMarketQuoteUserAlerts => StockMarketMemoryCache.StockMarketQuoteUserAlerts;
        public static IReadOnlyDictionary<int, StockMarketQuoteUserEmotionContract> StockMarketQuoteUserEmotions => StockMarketMemoryCache.StockMarketQuoteUserEmotions;
    }

    static class StockMarketMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, StockMarketExchangeContract> StockMarketExchanges = new();
        public static readonly ConcurrentDictionary<int, StockMarketTimeframeContract> StockMarketTimeframes = new();
        public static readonly ConcurrentDictionary<int, StockMarketAlertTypeContract> StockMarketAlertTypes = new();
        public static readonly ConcurrentDictionary<int, StockMarketAlertCompletedTypeContract> StockMarketAlertCompletedTypes = new();
        public static readonly ConcurrentDictionary<int, StockMarketEmotionContract> StockMarketEmotions = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteContract> StockMarketQuotes = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteUserAlertContract> StockMarketQuoteUserAlerts = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteUserEmotionContract> StockMarketQuoteUserEmotions = new();

        public static async Task LoadAsync(StockMarketRepository repository)
        {
            if (IsReady) return;

            var exchanges = (await repository.GetExchangesAsync()).Select(exchangeEntity => exchangeEntity.MaptToExchangeContract()).ToList();
            var timeframes = (await repository.GetTimeframesAsync()).Select(timeframeEntity => timeframeEntity.MaptToTimeframeContract()).ToList();
            var alertTypes = (await repository.GetAlertTypesAsync()).Select(alertTypeEntity => alertTypeEntity.MaptToAlertTypeContract()).ToList();
            var alertCompletedTypes = (await repository.GetAlertCompletedTypesAsync()).Select(alertCompletedTypeEntity => alertCompletedTypeEntity.MaptToAlertCompletedTypeContract()).ToList();
            var emotions = (await repository.GetEmotionsAsync()).Select(emotionEntity => emotionEntity.MapToEmotionContract()).ToList();
            var quotes = (await repository.GetQuotesAsync()).Select(quoteEntity => quoteEntity.MapToQuoteContract()).ToList();
            var quoteUserAlerts = (await repository.GetQuoteUserAlertsAsync()).Select(quoteUserAlertEntity => quoteUserAlertEntity.MapToQuoteUserAlertContract()).ToList();
            var quoteUserEmotions = (await repository.GetQuoteUserEmotionsAsync(DateTimeOffset.Now.AddDays(-1))).Select(quoteUserEmotionEntity => quoteUserEmotionEntity.MapToQuoteUserEmotionContract()).ToList();

            foreach (var exchange in exchanges)
                StockMarketExchanges[exchange.ExchangeId] = exchange;

            foreach (var timeframe in timeframes)
                StockMarketTimeframes[timeframe.TimeframeId] = timeframe;

            foreach (var alertType in alertTypes)
                StockMarketAlertTypes[alertType.AlertTypeId] = alertType;

            foreach (var alertCompletedType in alertCompletedTypes)
                StockMarketAlertCompletedTypes[alertCompletedType.AlertCompletedTypeId] = alertCompletedType;

            foreach (var emotion in emotions)
                StockMarketEmotions[emotion.EmotionId] = emotion;

            foreach (var quote in quotes)
                StockMarketQuotes[quote.QuoteId] = quote;

            foreach (var quoteUserAlert in quoteUserAlerts)
                StockMarketQuoteUserAlerts[quoteUserAlert.QuoteUserAlertId] = quoteUserAlert;

            foreach (var quoteUserEmotion in quoteUserEmotions)
                StockMarketQuoteUserEmotions[quoteUserEmotion.QuoteUserEmotionId] = quoteUserEmotion;

            IsReady = true;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task TrimAsync(StockMarketRepository repository)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            foreach (var quoteUserEmotion in StockMarketQuoteUserEmotions.Where(quoteUserEmotion => quoteUserEmotion.Value.CreatedOn < DateTimeOffset.Now.AddDays(-1)).ToList())
                StockMarketQuoteUserEmotions.TryRemove(quoteUserEmotion);

            await Task.CompletedTask;
        }
    }
}