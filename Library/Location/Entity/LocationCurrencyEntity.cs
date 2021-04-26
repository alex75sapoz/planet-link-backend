﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationCurrencyEntity
    {
        public LocationCurrencyEntity()
        {
            CountryCurrencies = new HashSet<LocationCountryCurrencyEntity>();
        }

        public int CurrencyId { get; internal set; }
        public string Name { get; internal set; }
        public string Symbol { get; internal set; }
        public string ThreeLetterCode { get; internal set; }
        public string ThreeDigitCode { get; internal set; }

        public virtual ICollection<LocationCountryCurrencyEntity> CountryCurrencies { get; internal set; }
    }
}

namespace Library.Location.Entity.Configuration
{
    public class CurrencyEntityConfiguration : IEntityTypeConfiguration<LocationCurrencyEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCurrencyEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationCurrencies));
            entity.HasKey(currency => currency.CurrencyId);

            entity.HasMany(currency => currency.CountryCurrencies).WithOne(countryCurrency => countryCurrency.Currency);
        }
    }
}