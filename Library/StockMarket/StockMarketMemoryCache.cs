using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.StockMarket
{
    public interface IStockMarketMemoryCache
    {
        public static IReadOnlyDictionary<int, StockMarketExchangeContract> Exchanges => StockMarketMemoryCache.Exchanges;
        public static IReadOnlyDictionary<int, StockMarketTimeframeContract> Timeframes => StockMarketMemoryCache.Timeframes;
        public static IReadOnlyDictionary<int, StockMarketAlertTypeContract> AlertTypes => StockMarketMemoryCache.AlertTypes;
        public static IReadOnlyDictionary<int, StockMarketAlertCompletedTypeContract> AlertCompletedTypes => StockMarketMemoryCache.AlertCompletedTypes;
        public static IReadOnlyDictionary<int, StockMarketEmotionContract> Emotions => StockMarketMemoryCache.Emotions;
        public static IReadOnlyDictionary<int, StockMarketQuoteContract> Quotes => StockMarketMemoryCache.Quotes;
        public static IReadOnlyDictionary<int, StockMarketQuoteUserAlertContract> QuoteUserAlerts => StockMarketMemoryCache.QuoteUserAlerts;
        public static IReadOnlyDictionary<int, StockMarketQuoteUserEmotionContract> QuoteUserEmotions => StockMarketMemoryCache.QuoteUserEmotions;
    }

    static class StockMarketMemoryCache
    {
        public static bool IsReady { get; private set; }

        public static readonly ConcurrentDictionary<int, StockMarketExchangeContract> Exchanges = new();
        public static readonly ConcurrentDictionary<int, StockMarketTimeframeContract> Timeframes = new();
        public static readonly ConcurrentDictionary<int, StockMarketAlertTypeContract> AlertTypes = new();
        public static readonly ConcurrentDictionary<int, StockMarketAlertCompletedTypeContract> AlertCompletedTypes = new();
        public static readonly ConcurrentDictionary<int, StockMarketEmotionContract> Emotions = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteContract> Quotes = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteUserAlertContract> QuoteUserAlerts = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteUserEmotionContract> QuoteUserEmotions = new();

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
                Exchanges[exchange.ExchangeId] = exchange;

            foreach (var timeframe in timeframes)
                Timeframes[timeframe.TimeframeId] = timeframe;

            foreach (var alertType in alertTypes)
                AlertTypes[alertType.AlertTypeId] = alertType;

            foreach (var alertCompletedType in alertCompletedTypes)
                AlertCompletedTypes[alertCompletedType.AlertCompletedTypeId] = alertCompletedType;

            foreach (var emotion in emotions)
                Emotions[emotion.EmotionId] = emotion;

            foreach (var quote in quotes)
                Quotes[quote.QuoteId] = quote;

            foreach (var quoteUserAlert in quoteUserAlerts)
                QuoteUserAlerts[quoteUserAlert.QuoteUserAlertId] = quoteUserAlert;

            foreach (var quoteUserEmotion in quoteUserEmotions)
                QuoteUserEmotions[quoteUserEmotion.QuoteUserEmotionId] = quoteUserEmotion;

            IsReady = true;
        }

#pragma warning disable IDE0060 // Remove unused parameter
        public static async Task TrimAsync(StockMarketRepository repository)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            foreach (var quoteUserEmotion in QuoteUserEmotions.Where(quoteUserEmotion => quoteUserEmotion.Value.CreatedOn < DateTimeOffset.Now.AddDays(-1)).ToList())
                QuoteUserEmotions.TryRemove(quoteUserEmotion);

            await Task.CompletedTask;
        }
    }
}