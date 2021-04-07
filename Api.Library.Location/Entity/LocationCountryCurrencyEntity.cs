using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Library.Location.Entity
{
    public class LocationCountryCurrencyEntity
    {
        public int CountryCurrencyId { get; internal set; }
        public int CountryId { get; internal set; }
        public int CurrencyId { get; internal set; }

        public virtual LocationCountryEntity Country { get; internal set; }
        public virtual LocationCurrencyEntity Currency { get; internal set; }
    }
}

namespace Api.Library.Location.Entity.Configuration
{
    internal class CountryCurrencyEntityConfiguration : IEntityTypeConfiguration<LocationCountryCurrencyEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCountryCurrencyEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationCountryCurrencies));
            entity.HasKey(countryCurrency => countryCurrency.CountryCurrencyId);

            entity.HasOne(countryCurrency => countryCurrency.Country).WithMany(country => country.CountryCurrencies).IsRequired(true);
            entity.HasOne(countryCurrency => countryCurrency.Currency).WithMany(country => country.CountryCurrencies).IsRequired(true);
        }
    }
}