using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationLanguageEntity
    {
        public LocationLanguageEntity()
        {
            CountryLanguages = new HashSet<LocationCountryLanguageEntity>();
        }

        public int LanguageId { get; internal set; }
        public string Name { get; internal set; }
        public string NativeName { get; internal set; }
        public string TwoLetterCode { get; internal set; }

        public virtual ICollection<LocationCountryLanguageEntity> CountryLanguages { get; internal set; }
    }
}

namespace Library.Location.Entity.Configuration
{
    internal class LanguageEntityConfiguration : IEntityTypeConfiguration<LocationLanguageEntity>
    {
        public void Configure(EntityTypeBuilder<LocationLanguageEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationLanguages));
            entity.HasKey(language => language.LanguageId);

            entity.HasMany(language => language.CountryLanguages).WithOne(countryLanguage => countryLanguage.Language);
        }
    }
}