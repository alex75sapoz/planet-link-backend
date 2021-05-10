using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingTechnologyStackEntity
    {
        public ProgrammingTechnologyStackEntity()
        {
            Projects = new HashSet<ProgrammingProjectEntity>();
        }

        public int TechnologyStackId { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<ProgrammingProjectEntity> Projects { get; internal set; }
    }
}

namespace Library.Programming.Entity.Configuration
{
    internal class ProgrammingTechnologyStackEntityConfiguration : IEntityTypeConfiguration<ProgrammingTechnologyStackEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingTechnologyStackEntity> entity)
        {
            entity.ToTable(nameof(ProgrammingContext.ProgrammingTechnologyStacks));
            entity.HasKey(technologyStack => technologyStack.TechnologyStackId);

            entity.HasMany(technologyStack => technologyStack.Projects).WithOne(project => project.TechnologyStack);
        }
    }
}