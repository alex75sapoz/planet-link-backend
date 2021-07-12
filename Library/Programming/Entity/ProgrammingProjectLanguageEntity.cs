using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Programming.Entity
{
    public class ProgrammingProjectLanguageEntity
    {
        public int ProjectLanguageId { get; internal set; }
        public int ProjectId { get; internal set; }
        public int LanguageId { get; internal set; }

        public virtual ProgrammingProjectEntity Project { get; internal set; } = default!;
        public virtual ProgrammingLanguageEntity Language { get; internal set; } = default!;
    }
}

namespace Library.Programming.Entity.Configuration
{
    class ProgrammingProjectLanguageEntityConfiguration : IEntityTypeConfiguration<ProgrammingProjectLanguageEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingProjectLanguageEntity> entity)
        {
            entity.ToTable(nameof(ProgrammingContext.ProgrammingProjectLanguages));
            entity.HasKey(projectLanguage => projectLanguage.ProjectLanguageId);

            entity.HasOne(projectLanguage => projectLanguage.Project).WithMany(project => project.ProjectLanguages).IsRequired(true);
            entity.HasOne(projectLanguage => projectLanguage.Language).WithMany(language => language.ProjectLanguages).IsRequired(true);
        }
    }
}