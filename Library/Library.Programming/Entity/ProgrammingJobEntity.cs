using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingJobEntity
    {
        public ProgrammingJobEntity()
        {
            Projects = new HashSet<ProgrammingProjectEntity>();
        }

        public int JobId { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<ProgrammingProjectEntity> Projects { get; internal set; }
    }
}

namespace Library.Programming.Entity.Configuration
{
    internal class ProgrammingJobEntityConfiguration : IEntityTypeConfiguration<ProgrammingJobEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingJobEntity> entity)
        {
            entity.ToTable(nameof(ProgrammingContext.ProgrammingJobs));
            entity.HasKey(job => job.JobId);

            entity.HasMany(job => job.Projects).WithOne(project => project.Job).HasForeignKey(project => project.JobId).IsRequired(true);
        }
    }
}