using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.Account.Entity
{
    public class AccountUserStocktwitsEntity
    {
        public int UserId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Username { get; internal set; } = default!;
        public int FollowersCount { get; internal set; }
        public int FollowingsCount { get; internal set; }
        public int PostsCount { get; internal set; }
        public int LikesCount { get; internal set; }
        public int WatchlistQuotesCount { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public virtual AccountUserEntity User { get; internal set; } = default!;
    }
}

namespace Library.Account.Entity.Configuration
{
    class AccountUserStocktwitsEntityConfiguration : IEntityTypeConfiguration<AccountUserStocktwitsEntity>
    {
        public void Configure(EntityTypeBuilder<AccountUserStocktwitsEntity> entity)
        {
            entity.ToTable(nameof(Account) + nameof(AccountContext.UsersStocktwits));
            entity.HasKey(userStocktwtits => userStocktwtits.UserId);

            entity.HasOne(userStocktwtits => userStocktwtits.User).WithOne(user => user.Stocktwits!);
        }
    }
}