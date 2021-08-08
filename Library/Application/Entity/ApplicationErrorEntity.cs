using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;

namespace Library.Application.Entity
{
    public class ApplicationErrorEntity
    {
        public int ErrorId { get; internal set; }
        public int ErrorTypeId { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public virtual ApplicationErrorTypeEntity ErrorType { get; internal set; } = default!;
        public virtual ApplicationErrorProcessingEntity? Processing { get; internal set; }
        public virtual ApplicationErrorRequestEntity? Request { get; internal set; }
        public virtual ApplicationErrorAuthenticationEntity? Authentication { get; internal set; }
    }
}

namespace Library.Application.Entity.Configuration
{
    class ApplicationErrorEntityConfiguration : IEntityTypeConfiguration<ApplicationErrorEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationErrorEntity> entity)
        {
            entity.ToTable(nameof(Application) + nameof(ApplicationContext.Errors));
            entity.HasKey(error => error.ErrorId);

            entity.HasOne(error => error.Processing).WithOne(errorProcessing => errorProcessing!.Error);
            entity.HasOne(error => error.Request).WithOne(errorRequest => errorRequest!.Error);
            entity.HasOne(error => error.Authentication).WithOne(errorAuthentication => errorAuthentication!.Error);

            entity.HasOne(error => error.ErrorType).WithMany(errorType => errorType.Errors);
        }
    }
}