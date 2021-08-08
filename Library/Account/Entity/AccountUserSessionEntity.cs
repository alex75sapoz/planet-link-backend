using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.Account.Entity
{
    public class AccountUserSessionEntity
    {
        public int UserSessionId { get; internal set; }
        public int UserId { get; internal set; }
        public string Token { get; internal set; } = default!;
        public string RefreshToken { get; internal set; } = default!;
        public DateTimeOffset TokenExpiresOn { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
        public DateTimeOffset LastUpdatedOn { get; internal set; }

        public virtual AccountUserEntity User { get; internal set; } = default!;
    }
}

namespace Library.Account.Entity.Configuration
{
    class AccountUserSessionEntityConfiguration : IEntityTypeConfiguration<AccountUserSessionEntity>
    {
        public void Configure(EntityTypeBuilder<AccountUserSessionEntity> entity)
        {
            entity.ToTable(nameof(Account) + nameof(AccountContext.UserSessions));
            entity.HasKey(userSession => userSession.UserSessionId);

            entity.HasOne(userSession => userSession.User).WithMany(user => user.UserSessions);
        }
    }
}