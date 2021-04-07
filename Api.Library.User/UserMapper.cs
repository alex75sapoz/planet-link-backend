using Api.Library.User.Contract;
using Api.Library.User.Entity;

namespace Api.Library.User
{
    internal static class UserMapper
    {
        public static UserTypeContract MapToTypeContract(this UserTypeEntity src) => new()
        {
            TypeId = src.UserTypeId,
            Name = src.Name
        };

        public static UserContract MapToUserContract(this UserEntity src) => new()
        {
            UserId = src.UserId,
            Google = src.Google?.MapToGoogleContract(),
            Stocktwits = src.Stocktwits?.MapToStocktwitsContract(),
            Type = UserMemoryCache.UserTypes[src.UserTypeId]
        };

        public static UserGoogleContract MapToGoogleContract(this UserGoogleEntity src) => new()
        {
            Name = src.Name,
            Username = src.Email
        };

        public static UserStocktwitsContract MapToStocktwitsContract(this UserStocktwitsEntity src) => new()
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

        public static UserSessionContract MapToSessionContract(this UserSessionEntity src) => new()
        {
            UserSessionId = src.UserSessionId,
            Token = src.Token,
            RefreshToken = src.RefreshToken,
            TokenExpiresOn = src.TokenExpiresOn,
            User = UserMemoryCache.Users[src.UserId]
        };
    }
}