using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Account.Entity
{
    public class AccountUserTypeEntity
    {
        public int UserTypeId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<AccountUserEntity> Users { get; internal set; } = new HashSet<AccountUserEntity>();
    }
}

namespace Library.Account.Entity.Configuration
{
    class AccountUserTypeEntityConfiguration : IEntityTypeConfiguration<AccountUserTypeEntity>
    {
        public void Configure(EntityTypeBuilder<AccountUserTypeEntity> entity)
        {
            entity.ToTable(nameof(Account) + nameof(AccountContext.UserTypes));
            entity.HasKey(userType => userType.UserTypeId);

            entity.HasMany(userType => userType.Users).WithOne(user => user.UserType);
        }
    }
}