using Api.Library.User.Contract;
using System.Collections.Generic;

namespace Api.Library.StockMarket.Contract
{
    public class StockMarketUserContract
    {
        public List<StockMarketUserAlertTypeCountContract> AlertTypeCounts { get; internal set; }
        public UserContract User { get; internal set; }
    }
}