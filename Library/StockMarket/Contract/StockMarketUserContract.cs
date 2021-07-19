using Library.Account;
using Library.Account.Contract;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Library.StockMarket.Contract
{
    public class StockMarketUserContract
    {
        [JsonIgnore]
        public int UserId { get; internal set; }

        public List<StockMarketUserAlertTypeCountContract> AlertTypeCounts { get; internal set; } = new List<StockMarketUserAlertTypeCountContract>();

        public AccountUserContract User => IAccountMemoryCache.Users[UserId];
    }
}