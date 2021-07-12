using RestSharp.Deserializers;
using System;

namespace Library.User.Response
{
    class StocktwitsUserResponseRoot
    {
        public UserStocktwitsResponse User { get; set; } = default!;
    }

    class UserStocktwitsResponse
    {
        [DeserializeAs(Name = "id")]
        public int UserId { get; set; }

        public string Name { get; set; } = default!;

        public string Username { get; set; } = default!;

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