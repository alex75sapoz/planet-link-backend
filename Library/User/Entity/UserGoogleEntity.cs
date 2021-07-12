using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.User.Entity
{
    public class UserGoogleEntity
    {
        public int UserId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Email { get; internal set; } = default!;

        public virtual UserEntity User { get; internal set; } = default!;
    }
}

namespace Library.User.Entity.Configuration
{
    class UserGoogleEntityConfiguration : IEntityTypeConfiguration<UserGoogleEntity>
    {
        public void Configure(EntityTypeBuilder<UserGoogleEntity> entity)
        {
            entity.ToTable(nameof(UserContext.UsersGoogle));
            entity.HasKey(userGoogle => userGoogle.UserId);

            entity.HasOne(google => google.User).WithOne(user => user.Google!).IsRequired(true);
        }
    }
}