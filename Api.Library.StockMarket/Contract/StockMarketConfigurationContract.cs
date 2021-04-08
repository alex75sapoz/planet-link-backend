using System.Collections.Generic;

namespace Api.Library.StockMarket.Contract
{
    public class StockMarketConfigurationContract
    {
        public List<StockMarketTimeframeContract> Timeframes { get; internal set; }
        public List<StockMarketAlertTypeContract> AlertTypes { get; internal set; }
        public List<StockMarketAlertCompletedTypeContract> AlertCompletedTypes { get; internal set; }
        public List<StockMarketExchangeContract> Exchanges { get; internal set; }
        public List<StockMarketEmotionContract> Emotions { get; internal set; }
        public StockMarketQuoteUserAlertRequirementConfigurationContract QuoteUserAlertRequirement { get; internal set; }
    }
}