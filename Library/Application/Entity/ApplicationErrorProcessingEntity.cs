using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Application.Entity
{
    public class ApplicationErrorProcessingEntity
    {
        public int ErrorId { get; internal set; }
        public string ClassName { get; internal set; } = default!;
        public string ClassMethodName { get; internal set; } = default!;
        public string ExceptionType { get; internal set; } = default!;
        public string ExceptionMessage { get; internal set; } = default!;
        public string? Input { get; internal set; }

        public virtual ApplicationErrorEntity Error { get; internal set; } = default!;
    }
}

namespace Library.Application.Entity.Configuration
{
    class ApplicationErrorProcessingEntityConfiguration : IEntityTypeConfiguration<ApplicationErrorProcessingEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationErrorProcessingEntity> entity)
        {
            entity.ToTable(nameof(Application) + nameof(ApplicationContext.ErrorsProcessing));
            entity.HasKey(errorProcessing => errorProcessing.ErrorId);

            entity.HasOne(errorProcessing => errorProcessing.Error).WithOne(error => error.Processing!);
        }
    }
}