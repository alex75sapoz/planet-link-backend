using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Error.Entity
{
    public class ErrorProcessingEntity
    {
        public int ErrorId { get; internal set; }
        public string ClassName { get; internal set; }
        public string ClassMethodName { get; internal set; }
        public string ExceptionType { get; internal set; }
        public string ExceptionMessage { get; internal set; }
        public string Input { get; internal set; }

        public virtual ErrorEntity Error { get; set; }
    }
}

namespace Library.Error.Entity.Configuration
{
    internal class ErrorProcessingEntityConfiguration : IEntityTypeConfiguration<ErrorProcessingEntity>
    {
        public void Configure(EntityTypeBuilder<ErrorProcessingEntity> entity)
        {
            entity.ToTable(nameof(ErrorContext.ErrorsProcessing));
            entity.HasKey(processing => processing.ErrorId);

            entity.HasOne(processing => processing.Error).WithOne(error => error.Processing).IsRequired(true);
        }
    }
}