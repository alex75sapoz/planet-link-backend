namespace Library.User
{
    public class UserConfiguration
    {
        public UserLimit Limit { get; set; } = default!;
        public UserDuration Duration { get; set; } = default!;
        public UserThreshold Threshold { get; set; } = default!;
        public UserDefault Default { get; set; } = default!;
        public UserGoogleApi GoogleApi { get; set; } = default!;
        public UserStocktwitsApi StocktwitsApi { get; set; } = default!;
        public UserFitbitApi FitbitApi { get; set; } = default!;

        public class UserLimit
        {
            public int SearchUsersLimit { get; set; }
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
            public string RedirectUrl { get; set; } = default!;
        }

        public class UserGoogleApi
        {
            public string AuthenticationServer { get; set; } = default!;
            public string TokenServer { get; set; } = default!;
            public string AuthenticationKey { get; set; } = default!;
            public string AuthenticationSecretKey { get; set; } = default!;
        }

        public class UserStocktwitsApi
        {
            public string Server { get; set; } = default!;
            public string AuthenticationKey { get; set; } = default!;
            public string AuthenticationSecretKey { get; set; } = default!;
        }

        public class UserFitbitApi
        {
            public string Server { get; set; } = default!;
            public string AuthenticationKey { get; set; } = default!;
            public string AuthenticationSecretKey { get; set; } = default!;
        }
    }
}