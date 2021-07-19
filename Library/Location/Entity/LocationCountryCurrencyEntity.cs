using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Location.Entity
{
    public class LocationCountryCurrencyEntity
    {
        public int CountryCurrencyId { get; internal set; }
        public int CountryId { get; internal set; }
        public int CurrencyId { get; internal set; }

        public virtual LocationCountryEntity Country { get; internal set; } = default!;
        public virtual LocationCurrencyEntity Currency { get; internal set; } = default!;
    }
}

namespace Library.Location.Entity.Configuration
{
    class LocationCountryCurrencyEntityConfiguration : IEntityTypeConfiguration<LocationCountryCurrencyEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCountryCurrencyEntity> entity)
        {
            entity.ToTable(nameof(Location) + nameof(LocationContext.CountryCurrencies));
            entity.HasKey(countryCurrency => countryCurrency.CountryCurrencyId);

            entity.HasOne(countryCurrency => countryCurrency.Country).WithMany(country => country.CountryCurrencies).IsRequired(true);
            entity.HasOne(countryCurrency => countryCurrency.Currency).WithMany(country => country.CountryCurrencies).IsRequired(true);
        }
    }
}