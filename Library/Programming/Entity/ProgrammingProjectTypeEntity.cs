using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingProjectTypeEntity
    {
        public int ProjectTypeId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<ProgrammingProjectEntity> Projects { get; internal set; } = new HashSet<ProgrammingProjectEntity>();
    }
}

namespace Library.Programming.Entity.Configuration
{
    class ProgrammingProjectTypeEntityConfiguration : IEntityTypeConfiguration<ProgrammingProjectTypeEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingProjectTypeEntity> entity)
        {
            entity.ToTable(nameof(Programming) + nameof(ProgrammingContext.ProjectTypes));
            entity.HasKey(projectType => projectType.ProjectTypeId);

            entity.HasMany(projectType => projectType.Projects).WithOne(project => project.ProjectType);
        }
    }
}