using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationAreaCodeEntity
    {
        public LocationAreaCodeEntity()
        {
            CountryAreaCodes = new HashSet<LocationCountryAreaCodeEntity>();
        }

        public int AreaCodeId { get; internal set; }
        public int CodeId { get; internal set; }

        public virtual ICollection<LocationCountryAreaCodeEntity> CountryAreaCodes { get; internal set; }
    }
}

namespace Library.Location.Entity.Configuration
{
    internal class LocationAreaCodeEntityConfiguration : IEntityTypeConfiguration<LocationAreaCodeEntity>
    {
        public void Configure(EntityTypeBuilder<LocationAreaCodeEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationAreaCodes));
            entity.HasKey(areaCode => areaCode.AreaCodeId);

            entity.HasMany(areaCode => areaCode.CountryAreaCodes).WithOne(countryAreaCode => countryAreaCode.AreaCode);
        }
    }
}