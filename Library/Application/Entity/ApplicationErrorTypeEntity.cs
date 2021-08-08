using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Application.Entity
{
    public class ApplicationErrorTypeEntity
    {
        public int ErrorTypeId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<ApplicationErrorEntity> Errors { get; internal set; } = new HashSet<ApplicationErrorEntity>();
    }
}

namespace Library.Application.Entity.Configuration
{
    class ApplicationErrorTypeEntityConfiguration : IEntityTypeConfiguration<ApplicationErrorTypeEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationErrorTypeEntity> entity)
        {
            entity.ToTable(nameof(Application) + nameof(ApplicationContext.ErrorTypes));
            entity.HasKey(errorType => errorType.ErrorTypeId);

            entity.HasMany(errorType => errorType.Errors).WithOne(error => error.ErrorType);
        }
    }
}