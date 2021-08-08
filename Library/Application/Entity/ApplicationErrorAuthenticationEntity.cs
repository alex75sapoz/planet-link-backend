using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Application.Entity
{
    public class ApplicationErrorAuthenticationEntity
    {
        public int ErrorId { get; set; }
        public string Method { get; internal set; } = default!;
        public string Path { get; internal set; } = default!;
        public string Query { get; internal set; } = default!;
        public string? TimezoneId { get; internal set; }
        public string? UserTypeId { get; internal set; }
        public string? Token { get; internal set; }
        public string? Code { get; internal set; }
        public string? Subdomain { get; internal set; }
        public string? Page { get; internal set; }
        public string ExceptionType { get; internal set; } = default!;
        public string ExceptionMessage { get; internal set; } = default!;

        public virtual ApplicationErrorEntity Error { get; internal set; } = default!;
    }
}

namespace Library.Application.Entity.Configuration
{
    class ApplicationErrorAuthenticationEntityConfiguration : IEntityTypeConfiguration<ApplicationErrorAuthenticationEntity>
    {
        public void Configure(EntityTypeBuilder<ApplicationErrorAuthenticationEntity> entity)
        {
            entity.ToTable(nameof(Application) + nameof(ApplicationContext.ErrorsAuthentication));
            entity.HasKey(errorAuthentication => errorAuthentication.ErrorId);

            entity.HasOne(errorAuthentication => errorAuthentication.Error).WithOne(error => error.Authentication!);
        }
    }
}