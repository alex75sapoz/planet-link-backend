using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationCurrencyEntity
    {
        public int CurrencyId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string Symbol { get; internal set; } = default!;
        public string ThreeLetterCode { get; internal set; } = default!;
        public string ThreeDigitCode { get; internal set; } = default!;

        public virtual ICollection<LocationCountryCurrencyEntity> CountryCurrencies { get; internal set; } = new HashSet<LocationCountryCurrencyEntity>();
    }
}

namespace Library.Location.Entity.Configuration
{
    public class LocationCurrencyEntityConfiguration : IEntityTypeConfiguration<LocationCurrencyEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCurrencyEntity> entity)
        {
            entity.ToTable(nameof(Location) + nameof(LocationContext.Currencies));
            entity.HasKey(currency => currency.CurrencyId);

            entity.HasMany(currency => currency.CountryCurrencies).WithOne(countryCurrency => countryCurrency.Currency);
        }
    }
}