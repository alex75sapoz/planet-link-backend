using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationCountryEntity
    {
        public int CountryId { get; internal set; }
        public int ContinentId { get; internal set; }
        public int SubContinentId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string TwoLetterCode { get; internal set; } = default!;
        public string ThreeLetterCode { get; internal set; } = default!;
        public int CapitalCityId { get; internal set; }

        public virtual LocationContinentEntity Continent { get; internal set; } = default!;
        public virtual LocationSubContinentEntity SubContinent { get; internal set; } = default!;
        public virtual ICollection<LocationCityEntity> Cities { get; internal set; } = new HashSet<LocationCityEntity>();
        public virtual ICollection<LocationCountryCurrencyEntity> CountryCurrencies { get; internal set; } = new HashSet<LocationCountryCurrencyEntity>();
        public virtual ICollection<LocationCountryLanguageEntity> CountryLanguages { get; internal set; } = new HashSet<LocationCountryLanguageEntity>();
        public virtual ICollection<LocationCountryAreaCodeEntity> CountryAreaCodes { get; internal set; } = new HashSet<LocationCountryAreaCodeEntity>();
    }
}

namespace Library.Location.Entity.Configuration
{
    class LocationCountryEntityConfiguration : IEntityTypeConfiguration<LocationCountryEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCountryEntity> entity)
        {
            entity.ToTable(nameof(Location) + nameof(LocationContext.Countries));
            entity.HasKey(country => country.CountryId);

            entity.HasOne(country => country.Continent).WithMany(continent => continent.Countries);
            entity.HasOne(country => country.SubContinent).WithMany(subContinent => subContinent.Countries);

            entity.HasMany(country => country.Cities).WithOne(city => city.Country);
            entity.HasMany(country => country.CountryCurrencies).WithOne(countryCurrency => countryCurrency.Country);
            entity.HasMany(country => country.CountryLanguages).WithOne(countryLanguage => countryLanguage.Country);
            entity.HasMany(country => country.CountryAreaCodes).WithOne(countryAreaCode => countryAreaCode.Country);
        }
    }
}