using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationStateEntity
    {
        public LocationStateEntity()
        {
            Cities = new HashSet<LocationCityEntity>();
        }

        public int StateId { get; internal set; }
        public string Name { get; internal set; }
        public string TwoLetterCode { get; internal set; }

        public virtual ICollection<LocationCityEntity> Cities { get; internal set; }
    }
}

namespace Library.Location.Entity.Configuration
{
    internal class StateEntityConfiguration : IEntityTypeConfiguration<LocationStateEntity>
    {
        public void Configure(EntityTypeBuilder<LocationStateEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationStates));
            entity.HasKey(state => state.StateId);

            entity.HasMany(state => state.Cities).WithOne(city => city.State);
        }
    }
}