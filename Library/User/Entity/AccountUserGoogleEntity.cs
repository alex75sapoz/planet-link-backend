using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Account.Entity
{
    public class AccountUserGoogleEntity
    {
        public int UserId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Email { get; internal set; } = default!;

        public virtual AccountUserEntity User { get; internal set; } = default!;
    }
}

namespace Library.Account.Entity.Configuration
{
    class AccountUserGoogleEntityConfiguration : IEntityTypeConfiguration<AccountUserGoogleEntity>
    {
        public void Configure(EntityTypeBuilder<AccountUserGoogleEntity> entity)
        {
            entity.ToTable(nameof(Account) + nameof(AccountContext.UsersGoogle));
            entity.HasKey(userGoogle => userGoogle.UserId);

            entity.HasOne(userGoogle => userGoogle.User).WithOne(user => user.Google!).IsRequired(true);
        }
    }
}