using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.Account.Entity
{
    public class AccountUserFitbitEntity
    {
        public int UserId { get; internal set; }
        public int UserGenderId { get; internal set; }
        public string FirstName { get; internal set; } = default!;
        public string LastName { get; internal set; } = default!;
        public int AgeInYears { get; internal set; }
        public decimal HeightInCentimeters { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public virtual AccountUserEntity User { get; set; } = default!;
        public virtual AccountUserGenderEntity UserGender { get; set; } = default!;
    }
}

namespace Library.Account.Entity.Configuration
{
    class AccountUserFitbitEntityConfiguration : IEntityTypeConfiguration<AccountUserFitbitEntity>
    {
        public void Configure(EntityTypeBuilder<AccountUserFitbitEntity> entity)
        {
            entity.ToTable(nameof(Account) + nameof(AccountContext.UsersFitbit));
            entity.HasKey(userFitbit => userFitbit.UserId);

            entity.HasOne(userFitbit => userFitbit.User).WithOne(user => user.Fitbit!);
            entity.HasOne(userFitbit => userFitbit.UserGender).WithMany(userGender => userGender.UsersFitbit);
        }
    }
}