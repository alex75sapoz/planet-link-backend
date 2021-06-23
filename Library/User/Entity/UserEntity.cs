using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Library.User.Entity
{
    public class UserEntity
    {
        public UserEntity()
        {
            ExternalUserId = default!;
            Type = default!;
            Sessions = new HashSet<UserSessionEntity>();
        }

        public int UserId { get; internal set; }
        public int UserTypeId { get; internal set; }
        public string ExternalUserId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
        public DateTimeOffset LastUpdatedOn { get; internal set; }

        public virtual UserTypeEntity Type { get; internal set; }
        public virtual UserGoogleEntity? Google { get; internal set; }
        public virtual UserStocktwitsEntity? Stocktwits { get; internal set; }
        public virtual ICollection<UserSessionEntity> Sessions { get; internal set; }
    }
}

namespace Library.User.Entity.Configuration
{
    internal class UserEntityConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> entity)
        {
            entity.ToTable(nameof(UserContext.Users));
            entity.HasKey(user => user.UserId);

            entity.HasOne(user => user.Google).WithOne(google => google!.User).IsRequired(false);
            entity.HasOne(user => user.Stocktwits).WithOne(google => google!.User).IsRequired(false);

            entity.HasOne(user => user.Type).WithMany(type => type.Users).HasForeignKey(user => user.UserTypeId).IsRequired(true);

            entity.HasMany(user => user.Sessions).WithOne(session => session.User).HasForeignKey(session => session.UserId);
        }
    }
}