using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Library.Location.Entity
{
    public class LocationCountryAreaCodeEntity
    {
        public int CountryAreaCodeId { get; internal set; }
        public int CountryId { get; internal set; }
        public int AreaCodeId { get; internal set; }

        public virtual LocationCountryEntity Country { get; internal set; } = default!;
        public virtual LocationAreaCodeEntity AreaCode { get; internal set; } = default!;
    }
}

namespace Library.Location.Entity.Configuration
{
    class CountryAreaCodeEntityConfiguration : IEntityTypeConfiguration<LocationCountryAreaCodeEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCountryAreaCodeEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationCountryAreaCodes));
            entity.HasKey(countryAreaCode => countryAreaCode.CountryAreaCodeId);

            entity.HasOne(countryAreaCode => countryAreaCode.Country).WithMany(country => country.CountryAreaCodes).IsRequired(true);
            entity.HasOne(countryAreaCode => countryAreaCode.Country).WithMany(country => country.CountryAreaCodes).IsRequired(true);
        }
    }
}