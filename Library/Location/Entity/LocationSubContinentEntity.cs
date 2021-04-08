using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationSubContinentEntity
    {
        public LocationSubContinentEntity()
        {
            Countries = new HashSet<LocationCountryEntity>();
        }

        public int SubContinentId { get; internal set; }
        public string Name { get; internal set; }

        public virtual ICollection<LocationCountryEntity> Countries { get; internal set; }
    }
}

namespace Library.Location.Entity.Configuration
{
    internal class SubContinentEntityConfiguration : IEntityTypeConfiguration<LocationSubContinentEntity>
    {
        public void Configure(EntityTypeBuilder<LocationSubContinentEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationSubContinents));
            entity.HasKey(subContinent => subContinent.SubContinentId);

            entity.HasMany(subContinent => subContinent.Countries).WithOne(country => country.SubContinent);
        }
    }
}