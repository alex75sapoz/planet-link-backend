using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.Error.Entity
{
    public class ErrorEntity
    {
        public ErrorEntity()
        {
            Type = default!;
        }

        public int ErrorId { get; internal set; }
        public int ErrorTypeId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public virtual ErrorTypeEntity Type { get; internal set; }
        public virtual ErrorProcessingEntity? Processing { get; internal set; }
        public virtual ErrorRequestEntity? Request { get; internal set; }
    }
}

namespace Library.Error.Entity.Configuration
{
    internal class ErrorEntityConfiguration : IEntityTypeConfiguration<ErrorEntity>
    {
        public void Configure(EntityTypeBuilder<ErrorEntity> entity)
        {
            entity.ToTable(nameof(ErrorContext.Errors));
            entity.HasKey(error => error.ErrorId);

            entity.HasOne(error => error.Processing).WithOne(processing => processing!.Error).IsRequired(false);
            entity.HasOne(error => error.Request).WithOne(request => request!.Error).IsRequired(false);

            entity.HasOne(error => error.Type).WithMany(type => type.Errors).HasForeignKey(error => error.ErrorTypeId).IsRequired(true);
        }
    }
}