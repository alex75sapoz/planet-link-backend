using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingLanguageEntity
    {
        public ProgrammingLanguageEntity()
        {
            ProjectLanguages = new HashSet<ProgrammingProjectLanguageEntity>();
        }

        public int LanguageId { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<ProgrammingProjectLanguageEntity> ProjectLanguages { get; internal set; }
    }
}

namespace Library.Programming.Entity.Configuration
{
    internal class ProgrammingLanguageEntityConfiguration : IEntityTypeConfiguration<ProgrammingLanguageEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingLanguageEntity> entity)
        {
            entity.ToTable(nameof(ProgrammingContext.ProgrammingLanguages));
            entity.HasKey(language => language.LanguageId);

            entity.HasMany(language => language.ProjectLanguages).WithOne(projectLanguage => projectLanguage.Language);
        }
    }
}