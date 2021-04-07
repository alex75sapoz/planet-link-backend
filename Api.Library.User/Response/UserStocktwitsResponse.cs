using RestSharp.Deserializers;
using System;

namespace Api.Library.User.Response
{
    internal class StocktwitsUserResponseRoot
    {
        public UserStocktwitsResponse User { get; set; }
    }

    internal class UserStocktwitsResponse
    {
        [DeserializeAs(Name = "id")]
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Username { get; set; }

        [DeserializeAs(Name = "followers")]
        public int FollowersCount { get; set; }

        [DeserializeAs(Name = "following")]
        public int FollowingsCount { get; set; }

        [DeserializeAs(Name = "ideas")]
        public int PostsCount { get; set; }

        [DeserializeAs(Name = "like_count")]
        public int LikesCount { get; set; }

        [DeserializeAs(Name = "watchlist_stocks_count")]
        public int WatchlistQuotesCount { get; set; }

        //Data provider returns date string without time or offset
        [DeserializeAs(Name = "join_date")]
        public DateTimeOffset CreatedOn { get; set; }
    }
}