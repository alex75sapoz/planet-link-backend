using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using NetTopologySuite.Geometries;

namespace Api.Library.Location.Entity
{
    public class LocationCityEntity
    {
        public int CityId { get; internal set; }
        public int CountryId { get; internal set; }
        public int? StateId { get; internal set; }
        public int OpenWeatherId { get; internal set; }
        public string Name { get; internal set; }
        public string County { get; internal set; }
        public string Zipcode { get; internal set; }
        public decimal Latitude { get; internal set; }
        public decimal Longitude { get; internal set; }
        public Point Point { get; internal set; }

        public virtual LocationStateEntity State { get; internal set; }
        public virtual LocationCountryEntity Country { get; internal set; }
    }
}

namespace Api.Library.Location.Entity.Configuration
{
    internal class CityEntityConfiguration : IEntityTypeConfiguration<LocationCityEntity>
    {
        public void Configure(EntityTypeBuilder<LocationCityEntity> entity)
        {
            entity.ToTable(nameof(LocationContext.LocationCities));
            entity.HasKey(city => city.CityId);

            entity.Property(city => city.Latitude).HasPrecision(18, 6);
            entity.Property(city => city.Longitude).HasPrecision(18, 6);

            entity.HasOne(city => city.State).WithMany(state => state.Cities);
            entity.HasOne(city => city.Country).WithMany(country => country.Cities);
        }
    }
}