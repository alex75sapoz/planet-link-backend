﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Location.Entity
{
    public class LocationCountryLanguageEntity
    {
        public int CountryLanguageId { get; internal set; }
        public int CountryId { get; internal set; }
        public int LanguageId { get; internal set; }

        public virtual LocationCountryEntity Country { get; internal set; } = default!;
        public virtual LocationLanguageEntity Language { get; internal set; } = default!;
    }
}

namespace Library.Location.Entity.Configuration
{
    class CountryLanguageEntityConfiguration : IEntityTypeConfiguration<LocationCountryLanguageEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCountryLanguageEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationCountryLanguages));
            entity.HasKey(countryLanguages => countryLanguages.CountryLanguageId);

            entity.HasOne(countryLanguage => countryLanguage.Country).WithMany(country => country.CountryLanguages).IsRequired(true);
            entity.HasOne(countryLanguage => countryLanguage.Language).WithMany(country => country.CountryLanguages).IsRequired(true);
        }
    }
}