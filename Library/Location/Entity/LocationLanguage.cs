using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationLanguageEntity
    {
        public int LanguageId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string NativeName { get; internal set; } = default!;
        public string TwoLetterCode { get; internal set; } = default!;

        public virtual ICollection<LocationCountryLanguageEntity> CountryLanguages { get; internal set; } = new HashSet<LocationCountryLanguageEntity>();
    }
}

namespace Library.Location.Entity.Configuration
{
    class LocationLanguageEntityConfiguration : IEntityTypeConfiguration<LocationLanguageEntity>
    {
        public void Configure(EntityTypeBuilder<LocationLanguageEntity> entity)
        {
            entity.ToTable(nameof(Location) + nameof(LocationContext.Languages));
            entity.HasKey(language => language.LanguageId);

            entity.HasMany(language => language.CountryLanguages).WithOne(countryLanguage => countryLanguage.Language);
        }
    }
}