using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Api.Library.Location.Entity
{
    public class LocationCountryAreaCodeEntity
    {
        public int CountryAreaCodeId { get; internal set; }
        public int CountryId { get; internal set; }
        public int AreaCodeId { get; internal set; }

        public virtual LocationCountryEntity Country { get; internal set; }
        public virtual LocationAreaCodeEntity AreaCode { get; internal set; }
    }
}

namespace Api.Library.Location.Entity.Configuration
{
    internal class CountryAreaCodeEntityConfiguration : IEntityTypeConfiguration<LocationCountryAreaCodeEntity>
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