using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Error.Entity
{
    public class ErrorTypeEntity
    {
        public ErrorTypeEntity()
        {
            Errors = new HashSet<ErrorEntity>();
        }

        public int ErrorTypeId { get; internal set; }
        public int Name { get; internal set; }

        public virtual ICollection<ErrorEntity> Errors { get; internal set; }
    }
}

namespace Library.Error.Entity.Configuration
{
    internal class ErrorTypeEntityConfiguration : IEntityTypeConfiguration<ErrorTypeEntity>
    {
        public void Configure(EntityTypeBuilder<ErrorTypeEntity> entity)
        {
            entity.ToTable(nameof(ErrorContext.ErrorTypes));
            entity.HasKey(type => type.ErrorTypeId);

            entity.HasMany(type => type.Errors).WithOne(error => error.Type).HasForeignKey(error => error.ErrorTypeId);
        }
    }
}