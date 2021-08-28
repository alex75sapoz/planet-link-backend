namespace Library.Account
{
    static class AccountMapper
    {
        public static AccountUserTypeContract MapToUserTypeContract(this AccountUserTypeEntity src) => new()
        {
            UserTypeId = src.UserTypeId,
            Name = src.Name
        };

        public static AccountUserGenderContract MapToUserGenderContract(this AccountUserGenderEntity src) => new()
        {
            UserGenderId = src.UserGenderId,
            Name = src.Name
        };

        public static AccountUserContract MapToUserContract(this AccountUserEntity src) => new()
        {
            UserTypeId = src.UserTypeId,
            UserId = src.UserId,
            IsAdministrator = src.IsAdministrator,
            Google = src.Google?.MapToUserGoogleContract(),
            Stocktwits = src.Stocktwits?.MapToUserStocktwitsContract(),
            Fitbit = src.Fitbit?.MapToUserFitbitContract()
        };

        public static AccountUserGoogleContract MapToUserGoogleContract(this AccountUserGoogleEntity src) => new()
        {
            Name = src.Name,
            Username = src.Email
        };

        public static AccountUserStocktwitsContract MapToUserStocktwitsContract(this AccountUserStocktwitsEntity src) => new()
        {
            Name = src.Name,
            Username = src.Username,
            FollowersCount = src.FollowersCount,
            FollowingsCount = src.FollowingsCount,
            LikesCount = src.LikesCount,
            PostsCount = src.PostsCount,
            WatchlistQuotesCount = src.WatchlistQuotesCount,
            CreatedOn = src.CreatedOn
        };

        public static AccountUserFitbitContract MapToUserFitbitContract(this AccountUserFitbitEntity src) => new()
        {
            UserGenderId = src.UserGenderId,
            FirstName = src.FirstName,
            LastName = src.LastName,
            AgeInYears = src.AgeInYears,
            HeightInCentimeters = src.HeightInCentimeters,
            CreatedOn = src.CreatedOn
        };

        public static AccountUserSessionContract MapToUserSessionContract(this AccountUserSessionEntity src) => new()
        {
            UserId = src.UserId,
            UserSessionId = src.UserSessionId,
            Token = src.Token,
            TokenExpiresOn = src.TokenExpiresOn
        };
    }
}