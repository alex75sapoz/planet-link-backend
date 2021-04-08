using Api.Library.StockMarket.Contract;
using Api.Library.StockMarket.Enum;
using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Api.Library.StockMarket
{
    internal static class StockMarketExtension
    {
        public static decimal GetChangePercent(decimal from, decimal to) =>
            from == 0
                ? from
                : (to - from) / from * 100;

        public static bool IsExchangeOpen(this StockMarketGlobalContract global, int exchangeId) => exchangeId switch
        {
            (int)Exchange.Nyse or
            (int)Exchange.Nasdaq or
            (int)Exchange.Amex or
            (int)Exchange.Index or
            (int)Exchange.Tsx or
            (int)Exchange.Commodity or
            (int)Exchange.MutualFund or
            (int)Exchange.Etf => global.IsStockMarketOpen,
            (int)Exchange.Crypto => global.IsCryptoMarketOpen,
            (int)Exchange.Forex => global.IsForexMarketOpen,
            (int)Exchange.Euronext => global.IsEuronextMarketOpen,
            _ => false,
        };

        public static List<StockMarketQuoteUserEmotionContract> GetQuoteUserEmotionsAtTimezoneToday(this IReadOnlyDictionary<int, StockMarketQuoteUserEmotionContract> quoteUserEmotions, DateTimeZone timezone, int? userId = null)
        {
            var datetimeOffsetAtTimezone = DateTimeOffset.Now.AtTimezone(timezone);

            return quoteUserEmotions
                .Where(quoteUserEmotion =>
                    (userId is null || quoteUserEmotion.Value.User.UserId == userId) &&
                    quoteUserEmotion.Value.CreatedOn.AtTimezone(timezone).Date == datetimeOffsetAtTimezone.Date
                )
                .Select(quoteUserEmotion => quoteUserEmotion.Value)
                .ToList();
        }
    }
}