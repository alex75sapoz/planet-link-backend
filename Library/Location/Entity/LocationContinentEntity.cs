using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationContinentEntity
    {
        public LocationContinentEntity()
        {
            Countries = new HashSet<LocationCountryEntity>();
        }

        public int ContinentId { get; internal set; }
        public string Name { get; internal set; }
        public string TwoLetterCode { get; internal set; }

        public virtual ICollection<LocationCountryEntity> Countries { get; internal set; }
    }
}

namespace Library.Location.Entity.Configuration
{
    internal class ContinentEntityConfiguration : IEntityTypeConfiguration<LocationContinentEntity>
    {
        public void Configure(EntityTypeBuilder<LocationContinentEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationContinents));
            entity.HasKey(continent => continent.ContinentId);

            entity.HasMany(continent => continent.Countries).WithOne(country => country.Continent);
        }
    }
}