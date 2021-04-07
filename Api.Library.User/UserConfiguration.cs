namespace Api.Library.User
{
    internal class UserConfiguration
    {
        public UserLimit Limit { get; set; }
        public UserDuration Duration { get; set; }
        public UserThreshold Threshold { get; set; }
        public UserDefault Default { get; set; }
        public UserGoogleApi GoogleApi { get; set; }
        public UserStocktwitsApi StocktwitsApi { get; set; }

        public class UserLimit
        {
            public int SearchStocktwitsLimit { get; set; }
        }

        public class UserDuration
        {
            public int StocktwitsTokenDurationInHours { get; set; }
        }

        public class UserThreshold
        {
            public int GoogleUpdateThresholdInHours { get; set; }
            public int StocktwitsUpdateThresholdInHours { get; set; }
        }

        public class UserDefault
        {
            public string RedirectUrl { get; set; }
        }

        public class UserGoogleApi
        {
            public string AuthenticationServer { get; set; }
            public string TokenServer { get; set; }
            public string AuthenticationKey { get; set; }
            public string AuthenticationSecretKey { get; set; }
        }

        public class UserStocktwitsApi
        {
            public string Server { get; set; }
            public string RedirectUrl { get; set; }
            public string AuthenticationKey { get; set; }
            public string AuthenticationSecretKey { get; set; }
        }
    }
}