using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.User.Entity
{
    public class UserGoogleEntity
    {
        public UserGoogleEntity()
        {
            Name = default!;
            Email = default!;
            User = default!;
        }

        public int UserId { get; internal set; }
        public string Name { get; internal set; }
        public string Email { get; internal set; }

        public virtual UserEntity User { get; internal set; }
    }
}

namespace Library.User.Entity.Configuration
{
    internal class UserGoogleEntityConfiguration : IEntityTypeConfiguration<UserGoogleEntity>
    {
        public void Configure(EntityTypeBuilder<UserGoogleEntity> entity)
        {
            entity.ToTable(nameof(UserContext.UsersGoogle));
            entity.HasKey(userGoogle => userGoogle.UserId);

            entity.HasOne(google => google.User).WithOne(user => user.Google!).IsRequired(true);
        }
    }
}