using Api.Library.Location.Entity;
using Microsoft.EntityFrameworkCore;

namespace Api.Library.Location
{
    internal class LocationContext : LibraryContext
    {
        public LocationContext(DbContextOptions<LocationContext> options) : base(options) { }

        public DbSet<LocationCityEntity> LocationCities { get; set; }
        public DbSet<LocationContinentEntity> LocationContinents { get; set; }
        public DbSet<LocationAreaCodeEntity> LocationAreaCodes { get; set; }
        public DbSet<LocationCurrencyEntity> LocationCurrencies { get; set; }
        public DbSet<LocationLanguageEntity> LocationLanguages { get; set; }
        public DbSet<LocationCountryAreaCodeEntity> LocationCountryAreaCodes { get; set; }
        public DbSet<LocationCountryCurrencyEntity> LocationCountryCurrencies { get; set; }
        public DbSet<LocationCountryEntity> LocationCountries { get; set; }
        public DbSet<LocationCountryLanguageEntity> LocationCountryLanguages { get; set; }
        public DbSet<LocationStateEntity> LocationStates { get; set; }
        public DbSet<LocationSubContinentEntity> LocationSubContinents { get; set; }
    }
}