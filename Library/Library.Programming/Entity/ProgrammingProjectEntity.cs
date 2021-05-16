using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingProjectEntity
    {
        public ProgrammingProjectEntity()
        {
            ProjectLanguages = new HashSet<ProgrammingProjectLanguageEntity>();
        }

        public int ProjectId { get; internal set; }
        public int ProjectTypeId { get; internal set; }
        public int JobId { get; internal set; }
        public int TechnologyStackId { get; internal set; }
        public string Name { get; internal set; }
        public string Tag { get; internal set; }
        public string Description { get; internal set; }
        public DateTimeOffset CreatedOn { get; internal set; }

        public virtual ProgrammingProjectTypeEntity Type { get; internal set; }
        public virtual ProgrammingJobEntity Job { get; internal set; }
        public virtual ProgrammingTechnologyStackEntity TechnologyStack { get; internal set; }
        public virtual ICollection<ProgrammingProjectLanguageEntity> ProjectLanguages { get; internal set; }
    }
}

namespace Library.Programming.Entity.Configuration
{
    internal class ProgrammingProjectEntityConfiguration : IEntityTypeConfiguration<ProgrammingProjectEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingProjectEntity> entity)
        {
            entity.ToTable(nameof(ProgrammingContext.ProgrammingProjects));
            entity.HasKey(project => project.ProjectId);

            entity.HasOne(project => project.Type).WithMany(type => type.Projects).HasForeignKey(project => project.ProjectTypeId).IsRequired(true);
            entity.HasOne(project => project.Job).WithMany(job => job.Projects).IsRequired(true);
            entity.HasOne(project => project.TechnologyStack).WithMany(technologyStack => technologyStack.Projects).IsRequired(true);

            entity.HasMany(project => project.ProjectLanguages).WithOne(projectLanguage => projectLanguage.Project);
        }
    }
}