﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingTechnologyStackEntity
    {
        public int TechnologyStackId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<ProgrammingProjectEntity> Projects { get; internal set; } = new HashSet<ProgrammingProjectEntity>();
    }
}

namespace Library.Programming.Entity.Configuration
{
    class ProgrammingTechnologyStackEntityConfiguration : IEntityTypeConfiguration<ProgrammingTechnologyStackEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingTechnologyStackEntity> entity)
        {
            entity.ToTable(nameof(Programming) + nameof(ProgrammingContext.TechnologyStacks));
            entity.HasKey(technologyStack => technologyStack.TechnologyStackId);

            entity.HasMany(technologyStack => technologyStack.Projects).WithOne(project => project.TechnologyStack);
        }
    }
}