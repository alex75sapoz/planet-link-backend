using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System.Collections.Generic;

namespace Library.Location.Entity
{
    public class LocationStateEntity
    {
        public int StateId { get; internal set; }
        public string Name { get; internal set; } = default!;
        public string TwoLetterCode { get; internal set; } = default!;

        public virtual ICollection<LocationCityEntity> Cities { get; internal set; } = new HashSet<LocationCityEntity>();
    }
}

namespace Library.Location.Entity.Configuration
{
    class StateEntityConfiguration : IEntityTypeConfiguration<LocationStateEntity>
    {
        public void Configure(EntityTypeBuilder<LocationStateEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationStates));
            entity.HasKey(state => state.StateId);

            entity.HasMany(state => state.Cities).WithOne(city => city.State!);
        }
    }
}