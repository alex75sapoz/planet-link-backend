using Microsoft.EntityFrameworkCore;

namespace Library.Error
{
    internal class ErrorContext : BaseContext
    {
        public ErrorContext(DbContextOptions<ErrorContext> options) : base(options)
        {
            ErrorTypes = default!;
            Errors = default!;
            ErrorsRequest = default!;
            ErrorsProcessing = default!;
        }

        public DbSet<ErrorTypeEntity> ErrorTypes { get; set; }
        public DbSet<ErrorEntity> Errors { get; set; }
        public DbSet<ErrorRequestEntity> ErrorsRequest { get; set; }
        public DbSet<ErrorProcessingEntity> ErrorsProcessing { get; set; }
    }
}