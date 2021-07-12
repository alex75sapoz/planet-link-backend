using Microsoft.EntityFrameworkCore;

namespace Library.Programming
{
    class ProgrammingContext : BaseContext
    {
        public ProgrammingContext(DbContextOptions<ProgrammingContext> options) : base(options) { }

        public DbSet<ProgrammingLanguageEntity> ProgrammingLanguages { get; set; } = default!;
        public DbSet<ProgrammingTechnologyStackEntity> ProgrammingTechnologyStacks { get; set; } = default!;
        public DbSet<ProgrammingJobEntity> ProgrammingJobs { get; set; } = default!;
        public DbSet<ProgrammingProjectTypeEntity> ProgrammingProjectTypes { get; set; } = default!;
        public DbSet<ProgrammingProjectEntity> ProgrammingProjects { get; set; } = default!;
        public DbSet<ProgrammingProjectLanguageEntity> ProgrammingProjectLanguages { get; set; } = default!;
    }
}