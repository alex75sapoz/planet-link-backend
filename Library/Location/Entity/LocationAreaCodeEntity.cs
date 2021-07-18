using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationAreaCodeEntity
    {
        public int AreaCodeId { get; internal set; }
        public int CodeId { get; internal set; }

        public virtual ICollection<LocationCountryAreaCodeEntity> CountryAreaCodes { get; internal set; } = new HashSet<LocationCountryAreaCodeEntity>();
    }
}

namespace Library.Location.Entity.Configuration
{
    class LocationAreaCodeEntityConfiguration : IEntityTypeConfiguration<LocationAreaCodeEntity>
    {
        public void Configure(EntityTypeBuilder<LocationAreaCodeEntity> entity)
        {
            entity.ToTable(nameof(Location) + nameof(LocationContext.AreaCodes));
            entity.HasKey(areaCode => areaCode.AreaCodeId);

            entity.HasMany(areaCode => areaCode.CountryAreaCodes).WithOne(countryAreaCode => countryAreaCode.AreaCode);
        }
    }
}