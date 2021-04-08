using Library.StockMarket.Contract;
using System.Collections.Concurrent;
using System.Collections.Generic;

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

    internal static class StockMarketMemoryCache
    {
        public static readonly ConcurrentDictionary<int, StockMarketExchangeContract> StockMarketExchanges = new();
        public static readonly ConcurrentDictionary<int, StockMarketTimeframeContract> StockMarketTimeframes = new();
        public static readonly ConcurrentDictionary<int, StockMarketAlertTypeContract> StockMarketAlertTypes = new();
        public static readonly ConcurrentDictionary<int, StockMarketAlertCompletedTypeContract> StockMarketAlertCompletedTypes = new();
        public static readonly ConcurrentDictionary<int, StockMarketEmotionContract> StockMarketEmotions = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteContract> StockMarketQuotes = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteUserAlertContract> StockMarketQuoteUserAlerts = new();
        public static readonly ConcurrentDictionary<int, StockMarketQuoteUserEmotionContract> StockMarketQuoteUserEmotions = new();
    }
}