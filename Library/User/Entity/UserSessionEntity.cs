using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.User.Entity
{
    public class UserSessionEntity
    {
        public int UserSessionId { get; internal set; }
        public int UserId { get; internal set; }
        public string Token { get; internal set; } = default!;
        public string RefreshToken { get; internal set; } = default!;
        public DateTimeOffset TokenExpiresOn { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }
        public DateTimeOffset LastUpdatedOn { get; internal set; }

        public virtual UserEntity User { get; internal set; } = default!;
    }
}

namespace Library.User.Entity.Configuration
{
    class UserSessionEntityConfiguration : IEntityTypeConfiguration<UserSessionEntity>
    {
        public void Configure(EntityTypeBuilder<UserSessionEntity> entity)
        {
            entity.ToTable(nameof(UserContext.UserSessions));
            entity.HasKey(userSession => userSession.UserSessionId);

            entity.HasOne(session => session.User).WithMany(user => user.Sessions).HasForeignKey(session => session.UserId).IsRequired(true);
        }
    }
}