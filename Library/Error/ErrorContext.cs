using Microsoft.EntityFrameworkCore;

namespace Library.Error
{
    class ErrorContext : BaseContext
    {
        public ErrorContext(DbContextOptions<ErrorContext> options) : base(options) { }

        public DbSet<ErrorTypeEntity> ErrorTypes { get; set; } = default!;
        public DbSet<ErrorEntity> Errors { get; set; } = default!;
        public DbSet<ErrorRequestEntity> ErrorsRequest { get; set; } = default!;
        public DbSet<ErrorProcessingEntity> ErrorsProcessing { get; set; } = default!;
    }
}