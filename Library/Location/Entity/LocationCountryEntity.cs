using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationCountryEntity
    {
        public LocationCountryEntity()
        {
            Cities = new HashSet<LocationCityEntity>();
            CountryCurrencies = new HashSet<LocationCountryCurrencyEntity>();
            CountryLanguages = new HashSet<LocationCountryLanguageEntity>();
            CountryAreaCodes = new HashSet<LocationCountryAreaCodeEntity>();
        }

        public int CountryId { get; internal set; }
        public int ContinentId { get; internal set; }
        public int SubContinentId { get; internal set; }
        public string Name { get; internal set; }
        public string TwoLetterCode { get; internal set; }
        public string ThreeLetterCode { get; internal set; }
        public int CapitalCityId { get; internal set; }

        public virtual LocationContinentEntity Continent { get; internal set; }
        public virtual LocationSubContinentEntity SubContinent { get; internal set; }
        public virtual ICollection<LocationCityEntity> Cities { get; internal set; }
        public virtual ICollection<LocationCountryCurrencyEntity> CountryCurrencies { get; internal set; }
        public virtual ICollection<LocationCountryLanguageEntity> CountryLanguages { get; internal set; }
        public virtual ICollection<LocationCountryAreaCodeEntity> CountryAreaCodes { get; internal set; }
    }
}

namespace Library.Location.Entity.Configuration
{
    internal class CountryEntityConfiguration : IEntityTypeConfiguration<LocationCountryEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCountryEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationCountries));
            entity.HasKey(country => country.CountryId);

            entity.HasOne(country => country.Continent).WithMany(continent => continent.Countries).IsRequired(true);
            entity.HasOne(country => country.SubContinent).WithMany(subContinent => subContinent.Countries).IsRequired(true);

            entity.HasMany(country => country.Cities).WithOne(city => city.Country);
            entity.HasMany(country => country.CountryCurrencies).WithOne(countryCurrency => countryCurrency.Country);
            entity.HasMany(country => country.CountryLanguages).WithOne(countryLanguage => countryLanguage.Country);
            entity.HasMany(country => country.CountryAreaCodes).WithOne(countryAreaCode => countryAreaCode.Country);
        }
    }
}