using Library.Base;
using Library.Programming.Entity;
using Microsoft.EntityFrameworkCore;

namespace Library.Programming
{
    internal class ProgrammingContext : BaseContext
    {
        public ProgrammingContext(DbContextOptions<ProgrammingContext> options) : base(options) { }

        public DbSet<ProgrammingLanguageEntity> ProgrammingLanguages { get; set; }
        public DbSet<ProgrammingTechnologyStackEntity> ProgrammingTechnologyStacks { get; set; }
        public DbSet<ProgrammingJobEntity> ProgrammingJobs { get; set; }
        public DbSet<ProgrammingProjectTypeEntity> ProgrammingProjectTypes { get; set; }
        public DbSet<ProgrammingProjectEntity> ProgrammingProjects { get; set; }
        public DbSet<ProgrammingProjectLanguageEntity> ProgrammingProjectLanguages { get; set; }
    }
}