using Microsoft.EntityFrameworkCore;

namespace Library.Application
{
    class ApplicationContext : BaseContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }

        public DbSet<ApplicationErrorTypeEntity> ErrorTypes { get; set; } = default!;
        public DbSet<ApplicationErrorEntity> Errors { get; set; } = default!;
        public DbSet<ApplicationErrorRequestEntity> ErrorsRequest { get; set; } = default!;
        public DbSet<ApplicationErrorProcessingEntity> ErrorsProcessing { get; set; } = default!;
    }
}