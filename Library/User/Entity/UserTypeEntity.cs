using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.User.Entity
{
    public class UserTypeEntity
    {
        public UserTypeEntity()
        {
            Name = default!;
            Users = new HashSet<UserEntity>();
        }

        public int UserTypeId { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<UserEntity> Users { get; internal set; }
    }
}

namespace Library.User.Entity.Configuration
{
    internal class UserTypeEntityConfiguration : IEntityTypeConfiguration<UserTypeEntity>
    {
        public void Configure(EntityTypeBuilder<UserTypeEntity> entity)
        {
            entity.ToTable(nameof(UserContext.UserTypes));
            entity.HasKey(type => type.UserTypeId);

            entity.HasMany(type => type.Users).WithOne(user => user.Type).HasForeignKey(user => user.UserTypeId);
        }
    }
}