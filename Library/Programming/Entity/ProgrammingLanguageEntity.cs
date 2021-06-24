using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Programming.Entity
{
    public class ProgrammingLanguageEntity
    {
        public int LanguageId { get; internal set; }
        public string Name { get; internal set; } = default!;

        public virtual ICollection<ProgrammingProjectLanguageEntity> ProjectLanguages { get; internal set; } = new HashSet<ProgrammingProjectLanguageEntity>();
    }
}

namespace Library.Programming.Entity.Configuration
{
    class ProgrammingLanguageEntityConfiguration : IEntityTypeConfiguration<ProgrammingLanguageEntity>
    {
        public void Configure(EntityTypeBuilder<ProgrammingLanguageEntity> entity)
        {
            entity.ToTable(nameof(ProgrammingContext.ProgrammingLanguages));
            entity.HasKey(language => language.LanguageId);

            entity.HasMany(language => language.ProjectLanguages).WithOne(projectLanguage => projectLanguage.Language);
        }
    }
}