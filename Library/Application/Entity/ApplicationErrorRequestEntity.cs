using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Application.Entity
{
    public class ApplicationErrorRequestEntity
    {
        public int ErrorId { get; internal set; }
        public string Method { get; internal set; } = default!;
        public string Path { get; internal set; } = default!;
        public string Query { get; internal set; } = default!;
        public int StatusCodeId { get; internal set; }
        public string ExceptionType { get; internal set; } = default!;
        public string ExceptionMessage { get; internal set; } = default!;
        public string TimezoneId { get; internal set; } = default!;
        public int? UserSessionId { get; internal set; }
        public int? UserId { get; internal set; }

        public virtual ApplicationErrorEntity Error { get; internal set; } = default!;
    }
}

namespace Library.Application.Entity.Configuration
{
    class ApplicationErrorRequestEntityConfiguration : IEntityTypeConfiguration<ApplicationErrorRequestEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationErrorRequestEntity> entity)
        {
            entity.ToTable(nameof(Application) + nameof(ApplicationContext.ErrorsRequest));
            entity.HasKey(errorRequest => errorRequest.ErrorId);

            entity.HasOne(errorRequest => errorRequest.Error).WithOne(error => error.Request!);
        }
    }
}