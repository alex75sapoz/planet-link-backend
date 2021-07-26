using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Library.Account.Entity
{
    public class AccountUserEntity
    {
        public int UserId { get; internal set; }
        public int UserTypeId { get; internal set; }
        public string ExternalUserId { get; internal set; } = default!;
        public DateTimeOffset CreatedOn { get; internal set; }
        public DateTimeOffset LastUpdatedOn { get; internal set; }

        public virtual AccountUserTypeEntity UserType { get; internal set; } = default!;
        public virtual AccountUserGoogleEntity? Google { get; internal set; }
        public virtual AccountUserStocktwitsEntity? Stocktwits { get; internal set; }
        public virtual AccountUserFitbitEntity? Fitbit { get; internal set; }
        public virtual ICollection<AccountUserSessionEntity> UserSessions { get; internal set; } = new HashSet<AccountUserSessionEntity>();
    }
}

namespace Library.Account.Entity.Configuration
{
    class AccountUserEntityConfiguration : IEntityTypeConfiguration<AccountUserEntity>
    {
        public void Configure(EntityTypeBuilder<AccountUserEntity> entity)
        {
            entity.ToTable(nameof(Account) + nameof(AccountContext.Users));
            entity.HasKey(user => user.UserId);

            entity.HasOne(user => user.Google).WithOne(google => google!.User);
            entity.HasOne(user => user.Stocktwits).WithOne(google => google!.User);
            entity.HasOne(user => user.Fitbit).WithOne(fitbit => fitbit!.User);

            entity.HasOne(user => user.UserType).WithMany(userType => userType.Users);

            entity.HasMany(user => user.UserSessions).WithOne(userSession => userSession.User);
        }
    }
}