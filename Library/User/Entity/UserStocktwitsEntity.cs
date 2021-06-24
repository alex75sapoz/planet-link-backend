using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.User.Entity
{
    public class UserStocktwitsEntity
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

        public virtual UserEntity User { get; internal set; } = default!;
    }
}

namespace Library.User.Entity.Configuration
{
    class UserStocktwitsEntityConfiguration : IEntityTypeConfiguration<UserStocktwitsEntity>
    {
        public void Configure(EntityTypeBuilder<UserStocktwitsEntity> entity)
        {
            entity.ToTable(nameof(UserContext.UsersStocktwits));
            entity.HasKey(stocktwtits => stocktwtits.UserId);

            entity.HasOne(stocktwits => stocktwits.User).WithOne(user => user.Stocktwits!).IsRequired(true);
        }
    }
}