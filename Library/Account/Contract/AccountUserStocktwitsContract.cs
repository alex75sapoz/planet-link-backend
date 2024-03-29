﻿using System;

namespace Library.Account.Contract
{
    public class AccountUserStocktwitsContract
    {
        public string Name { get; internal set; } = default!;
        public string Username { get; internal set; } = default!;
        public int FollowersCount { get; internal set; }
        public int FollowingsCount { get; internal set; }
        public int PostsCount { get; internal set; }
        public int LikesCount { get; internal set; }
        public int WatchlistQuotesCount { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
    }
}