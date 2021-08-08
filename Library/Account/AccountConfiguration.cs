namespace Library.Account
{
    public class AccountConfiguration
    {
        public AccountLimit Limit { get; set; } = default!;
        public AccountDuration Duration { get; set; } = default!;
        public AccountThreshold Threshold { get; set; } = default!;
        public AccountDefault Default { get; set; } = default!;
        public AccountGoogleApi GoogleApi { get; set; } = default!;
        public AccountStocktwitsApi StocktwitsApi { get; set; } = default!;
        public AccountFitbitApi FitbitApi { get; set; } = default!;

        public class AccountLimit
        {
            public int SearchUsersLimit { get; set; }
        }

        public class AccountDuration
        {
            public int StocktwitsTokenDurationInHours { get; set; }
        }

        public class AccountThreshold
        {
            public int GoogleUpdateThresholdInHours { get; set; }
            public int StocktwitsUpdateThresholdInHours { get; set; }
            public int FitbitUpdateThresholdInHours { get; set; }
        }

        public class AccountDefault
        {
            public string UserAuthenticationRedirectUrl { get; set; } = default!;
        }

        public class AccountGoogleApi
        {
            public string AuthenticationServer { get; set; } = default!;
            public string TokenServer { get; set; } = default!;
            public string AuthenticationKey { get; set; } = default!;
            public string AuthenticationSecretKey { get; set; } = default!;
        }

        public class AccountStocktwitsApi
        {
            public string Server { get; set; } = default!;
            public string AuthenticationKey { get; set; } = default!;
            public string AuthenticationSecretKey { get; set; } = default!;
        }

        public class AccountFitbitApi
        {
            public string AuthenticationServer { get; set; } = default!;
            public string TokenServer { get; set; } = default!;
            public string AuthenticationKey { get; set; } = default!;
            public string AuthenticationSecretKey { get; set; } = default!;
        }
    }
}