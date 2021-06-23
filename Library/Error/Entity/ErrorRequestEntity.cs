using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Error.Entity
{
    public class ErrorRequestEntity
    {
        public ErrorRequestEntity()
        {
            Method = default!;
            Path = default!;
            Query = default!;
            ExceptionType = default!;
            ExceptionMessage = default!;
            TimezoneId = default!;
            Error = default!;
        }

        public int ErrorId { get; internal set; }
        public string Method { get; internal set; }
        public string Path { get; internal set; }
        public string Query { get; internal set; }
        public int StatusCodeId { get; internal set; }
        public string ExceptionType { get; internal set; }
        public string ExceptionMessage { get; internal set; }
        public string TimezoneId { get; internal set; }
        public int? UserSessionId { get; internal set; }
        public int? UserId { get; internal set; }

        public virtual ErrorEntity Error { get; internal set; }
    }
}

namespace Library.Error.Entity.Configuration
{
    internal class ErrorRequestEntityConfiguration : IEntityTypeConfiguration<ErrorRequestEntity>
    {
        public void Configure(EntityTypeBuilder<ErrorRequestEntity> entity)
        {
            entity.ToTable(nameof(ErrorContext.ErrorsRequest));
            entity.HasKey(request => request.ErrorId);

            entity.HasOne(request => request.Error).WithOne(error => error.Request!).IsRequired(true);
        }
    }
}