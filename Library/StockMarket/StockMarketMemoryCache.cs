using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.StockMarket
{
    public interface IStockMarketMemoryCache
    {
        public static bool IsReady => StockMarketMemoryCache.IsReady;

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

        public static async Task RefreshAsync(StockMarketRepository repository)
        {
            var exchanges = (await repository.GetExchangesAsync()).Select(exchangeEntity => exchangeEntity.MaptToExchangeContract()).ToDictionary(exchange => exchange.ExchangeId);
            var timeframes = (await repository.GetTimeframesAsync()).Select(timeframeEntity => timeframeEntity.MaptToTimeframeContract()).ToDictionary(timeframe => timeframe.TimeframeId);
            var alertTypes = (await repository.GetAlertTypesAsync()).Select(alertTypeEntity => alertTypeEntity.MaptToAlertTypeContract()).ToDictionary(alertType => alertType.AlertTypeId);
            var alertCompletedTypes = (await repository.GetAlertCompletedTypesAsync()).Select(alertCompletedTypeEntity => alertCompletedTypeEntity.MaptToAlertCompletedTypeContract()).ToDictionary(alertCompletedType => alertCompletedType.AlertCompletedTypeId);
            var emotions = (await repository.GetEmotionsAsync()).Select(emotionEntity => emotionEntity.MapToEmotionContract()).ToDictionary(emotion => emotion.EmotionId);
            var quotes = (await repository.GetQuotesAsync()).Select(quoteEntity => quoteEntity.MapToQuoteContract()).ToDictionary(quote => quote.QuoteId);
            var quoteUserAlerts = (await repository.GetQuoteUserAlertsAsync()).Select(quoteUserAlertEntity => quoteUserAlertEntity.MapToQuoteUserAlertContract()).ToDictionary(quoteUserAlert => quoteUserAlert.QuoteUserAlertId);
            var quoteUserEmotions = (await repository.GetQuoteUserEmotionsAsync(DateTimeOffset.Now.AddDays(-1))).Select(quoteUserEmotionEntity => quoteUserEmotionEntity.MapToQuoteUserEmotionContract()).ToDictionary(quoteUserEmotion => quoteUserEmotion.QuoteUserEmotionId);

            //Exchanges
            foreach (var exchange in exchanges)
                StockMarketExchanges[exchange.Key] = exchange.Value;

            foreach (var exchange in StockMarketExchanges.Where(exchange => !exchanges.ContainsKey(exchange.Key)).ToList())
                StockMarketExchanges.TryRemove(exchange);

            //Timeframes
            foreach (var timeframe in timeframes)
                StockMarketTimeframes[timeframe.Key] = timeframe.Value;

            foreach (var timeframe in StockMarketTimeframes.Where(timeframe => !timeframes.ContainsKey(timeframe.Key)).ToList())
                StockMarketTimeframes.TryRemove(timeframe);

            //AlertTypes
            foreach (var alertType in alertTypes)
                StockMarketAlertTypes[alertType.Key] = alertType.Value;

            foreach (var alertType in StockMarketAlertTypes.Where(alertType => !alertTypes.ContainsKey(alertType.Key)).ToList())
                StockMarketAlertTypes.TryRemove(alertType);

            //AlertCompletedTypes
            foreach (var alertCompletedType in alertCompletedTypes)
                StockMarketAlertCompletedTypes[alertCompletedType.Key] = alertCompletedType.Value;

            foreach (var alertCompletedType in StockMarketAlertCompletedTypes.Where(alertCompletedType => !alertCompletedTypes.ContainsKey(alertCompletedType.Key)).ToList())
                StockMarketAlertCompletedTypes.TryRemove(alertCompletedType);

            //Emotions
            foreach (var emotion in emotions)
                StockMarketEmotions[emotion.Key] = emotion.Value;

            foreach (var emotion in StockMarketEmotions.Where(emotion => !emotions.ContainsKey(emotion.Key)).ToList())
                StockMarketEmotions.TryRemove(emotion);

            //Quotes
            foreach (var quote in quotes)
                StockMarketQuotes[quote.Key] = quote.Value;

            foreach (var quote in StockMarketQuotes.Where(quote => !quotes.ContainsKey(quote.Key)).ToList())
                StockMarketQuotes.TryRemove(quote);

            //QuoteUserAlerts
            foreach (var quoteUserAlert in quoteUserAlerts)
                StockMarketQuoteUserAlerts[quoteUserAlert.Key] = quoteUserAlert.Value;

            foreach (var quoteUserAlert in StockMarketQuoteUserAlerts.Where(quoteUserAlert => !quoteUserAlerts.ContainsKey(quoteUserAlert.Key)).ToList())
                StockMarketQuoteUserAlerts.TryRemove(quoteUserAlert);

            //QuoteUserEmotions
            foreach (var quoteUserEmotion in quoteUserEmotions)
                StockMarketQuoteUserEmotions[quoteUserEmotion.Key] = quoteUserEmotion.Value;

            foreach (var quoteUserEmotion in StockMarketQuoteUserEmotions.Where(quoteUserEmotion => !quoteUserEmotions.ContainsKey(quoteUserEmotion.Key)).ToList())
                StockMarketQuoteUserEmotions.TryRemove(quoteUserEmotion);

            IsReady = true;
        }
    }
}