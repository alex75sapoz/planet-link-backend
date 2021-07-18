using Microsoft.EntityFrameworkCore;

namespace Library.Programming
{
    class ProgrammingContext : BaseContext
    {
        public ProgrammingContext(DbContextOptions<ProgrammingContext> options) : base(options) { }

        public DbSet<ProgrammingLanguageEntity> Languages { get; set; } = default!;
        public DbSet<ProgrammingTechnologyStackEntity> TechnologyStacks { get; set; } = default!;
        public DbSet<ProgrammingJobEntity> Jobs { get; set; } = default!;
        public DbSet<ProgrammingProjectTypeEntity> ProjectTypes { get; set; } = default!;
        public DbSet<ProgrammingProjectEntity> Projects { get; set; } = default!;
        public DbSet<ProgrammingProjectLanguageEntity> ProjectLanguages { get; set; } = default!;
    }
}