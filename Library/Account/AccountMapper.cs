namespace Library.Account
{
    static class AccountMapper
    {
        public static AccountUserTypeContract MapToUserTypeContract(this AccountUserTypeEntity src) => new()
        {
            UserTypeId = src.UserTypeId,
            Name = src.Name
        };

        public static AccountUserContract MapToUserContract(this AccountUserEntity src) => new()
        {
            UserTypeId = src.UserTypeId,
            UserId = src.UserId,
            Google = src.Google?.MapToUserGoogleContract(),
            Stocktwits = src.Stocktwits?.MapToUserStocktwitsContract()
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

        public static AccountUserSessionContract MapToUserSessionContract(this AccountUserSessionEntity src) => new()
        {
            UserId = src.UserId,
            UserSessionId = src.UserSessionId,
            Token = src.Token,
            RefreshToken = src.RefreshToken,
            TokenExpiresOn = src.TokenExpiresOn
        };
    }
}