using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingProjectTypeEntity
    {
        public ProgrammingProjectTypeEntity()
        {
            Projects = new HashSet<ProgrammingProjectEntity>();
        }

        public int ProjectTypeId { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<ProgrammingProjectEntity> Projects { get; internal set; }
    }
}

namespace Library.Programming.Entity.Configuration
{
    internal class ProgrammingProjectTypeEntityConfiguration : IEntityTypeConfiguration<ProgrammingProjectTypeEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingProjectTypeEntity> entity)
        {
            entity.ToTable(nameof(ProgrammingContext.ProgrammingProjectTypes));
            entity.HasKey(projectType => projectType.ProjectTypeId);

            entity.HasMany(projectType => projectType.Projects).WithOne(project => project.Type).HasForeignKey(project => project.ProjectTypeId);
        }
    }
}