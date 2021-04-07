using Api.Library.Error.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Library.Error
{
    internal class ErrorContext : LibraryContext
    {
        public ErrorContext(DbContextOptions<ErrorContext> options) : base(options) { }

        public DbSet<ErrorTypeEntity> ErrorTypes { get; set; }
        public DbSet<ErrorEntity> Errors { get; set; }
        public DbSet<ErrorRequestEntity> ErrorsRequest { get; set; }
        public DbSet<ErrorProcessingEntity> ErrorsProcessing { get; set; }
    }
}