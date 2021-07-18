using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationContinentEntity
    {
        public int ContinentId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string TwoLetterCode { get; internal set; } = default!;

        public virtual ICollection<LocationCountryEntity> Countries { get; internal set; } = new HashSet<LocationCountryEntity>();
    }
}

namespace Library.Location.Entity.Configuration
{
    class LocationContinentEntityConfiguration : IEntityTypeConfiguration<LocationContinentEntity>
    {
        public void Configure(EntityTypeBuilder<LocationContinentEntity> entity)
        {
            entity.ToTable(nameof(Location) + nameof(LocationContext.Continents));
            entity.HasKey(continent => continent.ContinentId);

            entity.HasMany(continent => continent.Countries).WithOne(country => country.Continent);
        }
    }
}