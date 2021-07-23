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
    class LocationCountryAreaCodeEntityConfiguration : IEntityTypeConfiguration<LocationCountryAreaCodeEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCountryAreaCodeEntity> entity)
        {
            entity.ToTable(nameof(Location) + nameof(LocationContext.CountryAreaCodes));
            entity.HasKey(countryAreaCode => countryAreaCode.CountryAreaCodeId);

            entity.HasOne(countryAreaCode => countryAreaCode.Country).WithMany(country => country.CountryAreaCodes);
            entity.HasOne(countryAreaCode => countryAreaCode.Country).WithMany(country => country.CountryAreaCodes);
        }
    }
}