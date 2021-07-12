using System.Collections.Generic;

namespace Library.StockMarket.Contract
{
    public class StockMarketConfigurationContract
    {
        public List<StockMarketTimeframeContract> Timeframes { get; internal set; } = default!;
        public List<StockMarketAlertTypeContract> AlertTypes { get; internal set; } = default!;
        public List<StockMarketAlertCompletedTypeContract> AlertCompletedTypes { get; internal set; } = default!;
        public List<StockMarketExchangeContract> Exchanges { get; internal set; } = default!;
        public List<StockMarketEmotionContract> Emotions { get; internal set; } = default!;
        public StockMarketQuoteUserAlertRequirementConfigurationContract QuoteUserAlertRequirement { get; internal set; } = default!;
    }
}