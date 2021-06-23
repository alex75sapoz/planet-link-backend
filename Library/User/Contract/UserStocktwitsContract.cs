using System;

namespace Library.User.Contract
{
    public class UserStocktwitsContract
    {
        public UserStocktwitsContract()
        {
            Name = default!;
            Username = default!;
        }

        public string Name { get; internal set; }
        public string Username { get; internal set; }
        public int FollowersCount { get; internal set; }
        public int FollowingsCount { get; internal set; }
        public int PostsCount { get; internal set; }
        public int LikesCount { get; internal set; }
        public int WatchlistQuotesCount { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
    }
}