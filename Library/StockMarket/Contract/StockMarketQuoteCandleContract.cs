using System;
using System.Collections.Generic;

namespace Library.StockMarket.Contract
{
    public class StockMarketQuoteCandleContract
    {
        public decimal Open { get; internal set; }
        public decimal Low { get; internal set; }
        public decimal High { get; internal set; }
        public decimal Close { get; internal set; }
        public long? Volume { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
        public int TimeframeMultiplier { get; internal set; }

        public List<StockMarketQuoteUserAlertContract> QuoteUserAlerts { get; internal set; }
    }
}