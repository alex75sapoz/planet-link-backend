using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingJobEntity
    {
        public int JobId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<ProgrammingProjectEntity> Projects { get; internal set; } = new HashSet<ProgrammingProjectEntity>();
    }
}

namespace Library.Programming.Entity.Configuration
{
    class ProgrammingJobEntityConfiguration : IEntityTypeConfiguration<ProgrammingJobEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingJobEntity> entity)
        {
            entity.ToTable(nameof(Programming) + nameof(ProgrammingContext.Jobs));
            entity.HasKey(job => job.JobId);

            entity.HasMany(job => job.Projects).WithOne(project => project.Job);
        }
    }
}