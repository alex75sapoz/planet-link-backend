using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Account.Entity
{
    public class AccountUserGenderEntity
    {
        public int UserGenderId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<AccountUserFitbitEntity> UsersFitbit { get; set; } = new HashSet<AccountUserFitbitEntity>();
    }
}

namespace Library.Account.Entity.Configuration
{
    class AccountUserGenderEntityConfiguration : IEntityTypeConfiguration<AccountUserGenderEntity>
    {
        public void Configure(EntityTypeBuilder<AccountUserGenderEntity> entity)
        {
            entity.ToTable(nameof(Account) + nameof(AccountContext.UserGenders));
            entity.HasKey(userGender => userGender.UserGenderId);

            entity.HasMany(userGender => userGender.UsersFitbit).WithOne(userFitbit => userFitbit.UserGender);
        }
    }
}