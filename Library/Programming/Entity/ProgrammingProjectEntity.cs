﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingProjectEntity
    {
        public int ProjectId { get; internal set; }
        public int ProjectTypeId { get; internal set; }
        public int JobId { get; internal set; }
        public int TechnologyStackId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Tag { get; internal set; } = default!;
        public string Description { get; internal set; } = default!;
        public bool IsImportant { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public virtual ProgrammingProjectTypeEntity ProjectType { get; internal set; } = default!;
        public virtual ProgrammingJobEntity Job { get; internal set; } = default!;
        public virtual ProgrammingTechnologyStackEntity TechnologyStack { get; internal set; } = default!;
        public virtual ICollection<ProgrammingProjectLanguageEntity> ProjectLanguages { get; internal set; } = new HashSet<ProgrammingProjectLanguageEntity>();
    }
}

namespace Library.Programming.Entity.Configuration
{
    class ProgrammingProjectEntityConfiguration : IEntityTypeConfiguration<ProgrammingProjectEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingProjectEntity> entity)
        {
            entity.ToTable(nameof(Programming) + nameof(ProgrammingContext.Projects));
            entity.HasKey(project => project.ProjectId);

            entity.HasOne(project => project.ProjectType).WithMany(projectType => projectType.Projects);
            entity.HasOne(project => project.Job).WithMany(job => job.Projects);
            entity.HasOne(project => project.TechnologyStack).WithMany(technologyStack => technologyStack.Projects);

            entity.HasMany(project => project.ProjectLanguages).WithOne(projectLanguage => projectLanguage.Project);
        }
    }
}